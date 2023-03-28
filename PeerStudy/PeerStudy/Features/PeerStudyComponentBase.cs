using Blazored.Toast.Configuration;
using Blazored.Toast.Services;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features
{
    public abstract class PeerStudyComponentBase : FluxorComponent
    {
        [Inject]
        protected IAuthService AuthService { get; set; }

        [Inject]
        protected INavigationMenuService NavigationMenuService { get; set; }

        [Inject]
        protected IToastService ToastService { get; set; }

        protected Guid currentUserId;
        protected Role? currentUserRole;
        protected string userEmail;
        protected bool isLoading;

        protected abstract Task InitializeAsync();

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await InitializeAsync();
            isLoading = false;
        }

        protected async Task SetCurrentUserDataAsync()
        {
            currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
            string userRole = await AuthService.GetCurrentUserRole();
            currentUserRole = GetCurrentUserRole(userRole);
            userEmail = await AuthService.GetCurrentUserEmailAsync();
        }

        protected void ResetNavigationBar()
        {
            NavigationMenuService.Reset();
        }

        //TODO: move this to a dedicated service to use it whenever needed
        protected void ShowToast(ToastLevel type, string message, bool autoClose = true)
        {
            ToastService.ClearAll();
            Action<ToastSettings>? action = null;

            if (!autoClose)
            {
                action = (settings) => settings.DisableTimeout = true;
            }

            if (type == ToastLevel.Error)
            {
                ToastService.ShowError(message, action);
            }
            else if (type == ToastLevel.Info)
            {
                ToastService.ShowInfo(message, action);
            }
            else if (type == ToastLevel.Success)
            {
                ToastService.ShowSuccess(message, action);
            }
        }

        private Role? GetCurrentUserRole(string userRole)
        {
            Enum.TryParse(typeof(Role), userRole, out var currentUserRole);

            return (Role?)currentUserRole;
        }
    }
}
