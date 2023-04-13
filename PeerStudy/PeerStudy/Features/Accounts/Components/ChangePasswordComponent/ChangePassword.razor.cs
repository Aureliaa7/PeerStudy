using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Accounts;
using PeerStudy.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Accounts.Components.ChangePasswordComponent
{
    public partial class ChangePassword
    {
        [Inject]
        private IAccountService AccountService { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }


        private ChangePasswordModel changePasswordModel = new();
        private string styleRules = "width: 80%";
        private Guid userId;
        private bool isBtnDisabled;

        protected override async Task OnInitializedAsync()
        {
            userId = new Guid(await AuthService.GetCurrentUserIdAsync());
        }

        private async Task UpdatePassword()
        {
            isBtnDisabled = true;
            StateHasChanged();

            changePasswordModel.UserId = userId;
            var result = await AccountService.ChangePasswordAsync(changePasswordModel);
            changePasswordModel = new();
            if (result)
            {
                ToastService.ShowToast(ToastLevel.Success, "Password successfully changed.");
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, "The password could not be changed...");
            }

            isBtnDisabled = false;
            StateHasChanged();
        }
    }
}
