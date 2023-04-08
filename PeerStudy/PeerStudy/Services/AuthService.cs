using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models;
using PeerStudy.Infrastructure;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerStudy.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService localStorageService;
        private readonly IAccountService accountService;
        private readonly AuthenticationStateProvider authStateProvider;
        private readonly ICacheService cacheService;
        private readonly INavigationMenuService navigationMenuService;

        public AuthService(
            ILocalStorageService localStorageService,
            IAccountService accountService,
            AuthenticationStateProvider authStateProvider,
            ICacheService cacheService,
            INavigationMenuService navigationMenuService)
        {
            this.localStorageService = localStorageService;
            this.accountService = accountService;
            this.authStateProvider = authStateProvider;
            this.cacheService = cacheService;
            this.navigationMenuService = navigationMenuService;
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

                navigationMenuService.CurrentUsername = JwtHelper.GetClaimValueByName(token, ClaimTypes.Name);
              
                return JwtHelper.GetClaimValueByName(token, Constants.UserId);
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            await ClearCacheAsync();
            navigationMenuService.Reset();

            navigationMenuService.CurrentUsername = null;
            await localStorageService.RemoveItemAsync(ClientConstants.Token);
            ((PeerStudyAuthStateProvider)authStateProvider).NotifyUserLogout();
        }

        public Task<string> GetCurrentUserRole()
        {
            return GetClaimByNameAsync(ClaimTypes.Role);
        }

        public Task<string> GetCurrentUserEmailAsync()
        {
            return GetClaimByNameAsync(ClaimTypes.Email);
        }

        private async Task ClearCacheAsync()
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var keysToBeRemoved = GetCacheKeysByUser(currentUserId);
            await cacheService.RemoveByKeysAsync(keysToBeRemoved);
        }

        private List<string> GetCacheKeysByUser(string userId)
        {
            return ClientConstants.CacheConstants.Select(x => $"{userId}_{x}").ToList();
        }
    }
}
