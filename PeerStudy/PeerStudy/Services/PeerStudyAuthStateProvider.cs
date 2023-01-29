using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PeerStudy.Infrastructure.Helpers;
using System;
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
            try
            {
                string token = await localStorageService.GetItemAsStringAsync(ClientConstants.Token);
                bool isJwtValid = JwtHelper.IsValidJwt(token);

                if (!string.IsNullOrEmpty(token) && isJwtValid)
                {
                    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtHelper.GetClaimsFromJWT(token), "jwt"));
                    var authState = new AuthenticationState(authenticatedUser);

                    NotifyAuthenticationStateChanged(Task.FromResult(authState));

                    return authState;
                }

                await localStorageService.RemoveItemAsync(ClientConstants.Token);
                return GetUnauthorizedState();
            }
            catch (Exception)
            {
                return GetUnauthorizedState();
            }
        }

        private AuthenticationState GetUnauthorizedState()
        {
            var unauthorizedState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            // if the user is not authenticated, notify the UI
            NotifyAuthenticationStateChanged(Task.FromResult(unauthorizedState));

            return unauthorizedState;
        }

        /// <summary>
        /// Needed bc. the component isn't notified automatically 
        /// if the underlying authentication state data changes.
        /// </summary>
        /// <param name="token"></param>
        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtHelper.GetClaimsFromJWT(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

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
