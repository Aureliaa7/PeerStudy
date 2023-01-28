using Blazorise;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Components.Accounts
{
    public partial class ChangePassword
    {
        [Inject]
        private IAccountService AccountService { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }

        private ChangePasswordModel changePasswordModel = new();

        private Color alertColor; 
        private string alertText;
        private bool isAlertVisible;
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
            isAlertVisible = true;
            changePasswordModel = new();
            if (result)
            {
                alertColor = Color.Success;
                alertText = "Password successfully changed.";
            }
            else
            {
                alertColor = Color.Danger;
                alertText = "The password could not be changed...";
            }

            isBtnDisabled = false;
            StateHasChanged();
        }
    }
}
