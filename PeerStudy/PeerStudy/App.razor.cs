using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy
{
    public partial class App
    {
        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private INavigationMenuService NavigationMenuService { get; set; }

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                NavigationMenuService.CurrentUsername = await AuthService.GetCurrentUserNameAsync();
                NavigationMenuService.CurrentUserProfileImage = await AuthService.GetCurrentUserProfilePhotoNameAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while updating the navigation bar...");
            }
        }
    }
}
