using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PeerStudy.Infrastructure.Helpers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerStudy.Services
{
    public class PeerStudyAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorageService;

        public PeerStudyAuthStateProvider(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await localStorageService.GetItemAsStringAsync(ClientConstants.Token);
            bool isJwtValid = JwtHelper.IsValidJwt(token);

            if (!string.IsNullOrEmpty(token) && isJwtValid)
            {
                return new AuthenticationState(GetClaimsPrincipal(token));
            }

            // when removing the jwt from local storage, notify the UI
            await localStorageService.RemoveItemAsync(ClientConstants.Token);
            var newAuthState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(newAuthState));

            return newAuthState;
        }

        /// <summary>
        /// Needed bc. the component isn't notified automatically 
        /// if the underlying authentication state data changes.
        /// </summary>
        /// <param name="token"></param>
        public void NotifyUserAuthentication(string token)
        {
            var claimsPrincipal = GetClaimsPrincipal(token);
            var authState = Task.FromResult(new AuthenticationState(claimsPrincipal));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(
                new AuthenticationState(
                    new ClaimsPrincipal(
                        new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }

        private static ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            var claims = JwtHelper.GetClaimsFromJWT(token);
            if (claims.Any())
            {
                var claimsIdentity = new ClaimsIdentity(claims, "jwtAuthType");
                return new ClaimsPrincipal(claimsIdentity);
            }

            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
