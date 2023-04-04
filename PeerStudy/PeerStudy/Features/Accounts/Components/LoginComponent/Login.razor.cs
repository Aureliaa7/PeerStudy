using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models;
using PeerStudy.Services.Interfaces;
using System.Threading.Tasks;

namespace PeerStudy.Features.Accounts.Components.LoginComponent
{
    public partial class Login
    {
        [Inject]
        private IAuthService AuthenticationService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }


        private LoginModel loginModel = new();
        private bool isSubmitButtonDisabled;
        private string styleRules = "width: 80%";

        private async Task LogIn()
        {
            DisableSubmitButton(true);
            string userId = await AuthenticationService.LoginAsync(loginModel);
            DisableSubmitButton(false);

            if (string.IsNullOrEmpty(userId))
            {
                ToastService.ShowToast(ToastLevel.Error, "Failed login!");
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }

            loginModel = new();
        }

        private void DisableSubmitButton(bool disabled)
        {
            isSubmitButtonDisabled = disabled;
            StateHasChanged();
        }
    }
}
