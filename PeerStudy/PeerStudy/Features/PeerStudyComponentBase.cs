using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features
{
    public abstract class PeerStudyComponentBase : ComponentBase
    {
        [Inject]
        protected IAuthService AuthService { get; set; }

        [Inject]
        protected INavigationMenuService NavigationMenuService { get; set; }

        [Inject]
        protected IPeerStudyToastService ToastService { get; set; }

        protected Guid currentUserId;
        protected Role? currentUserRole;
        protected string userEmail;
        protected string userName;
        protected bool isLoading;

        protected abstract Task InitializeAsync();

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                await InitializeAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred...");
            }
            isLoading = false;
        }

        protected async Task SetCurrentUserDataAsync()
        {
            currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
            string userRole = await AuthService.GetCurrentUserRole();
            currentUserRole = GetCurrentUserRole(userRole);
            userEmail = await AuthService.GetCurrentUserEmailAsync();
            userName = await AuthService.GetCurrentUserNameAsync();
        }

        protected void ResetNavigationBar()
        {
            NavigationMenuService.Reset();
        }

        private Role? GetCurrentUserRole(string userRole)
        {
            Enum.TryParse(typeof(Role), userRole, out var currentUserRole);

            return (Role?)currentUserRole;
        }
    }
}
