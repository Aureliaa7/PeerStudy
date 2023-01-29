using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models;
using PeerStudy.Infrastructure;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerStudy.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService localStorageService;
        private readonly IAccountService accountService;
        private readonly AuthenticationStateProvider authStateProvider;

        public AuthService(
            ILocalStorageService localStorageService,
            IAccountService accountService,
            AuthenticationStateProvider authStateProvider)
        {
            this.localStorageService = localStorageService;
            this.accountService = accountService;
            this.authStateProvider = authStateProvider;
        }

        public async Task<string> GetCurrentUserFullNameAsync()
        {
            return await GetClaimByNameAsync(Constants.UserFullName);
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            return await GetClaimByNameAsync(Constants.UserId);
        }

        private async Task<string> GetClaimByNameAsync(string name)
        {
            var token = await localStorageService.GetItemAsStringAsync(ClientConstants.Token);

            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return JwtHelper.GetClaimValueByName(token, name);
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            string token = await accountService.LoginAsync(loginModel);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await localStorageService.SetItemAsStringAsync(ClientConstants.Token, token);
                ((PeerStudyAuthStateProvider)authStateProvider).NotifyUserAuthentication(token);

                return JwtHelper.GetClaimValueByName(token, Constants.UserId);
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            await localStorageService.RemoveItemAsync(ClientConstants.Token);
            ((PeerStudyAuthStateProvider)authStateProvider).NotifyUserLogout();
        }
    }
}
