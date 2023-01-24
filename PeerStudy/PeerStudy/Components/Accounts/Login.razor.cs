using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models;
using PeerStudy.Services.Interfaces;
using System.Threading.Tasks;

namespace PeerStudy.Components.Accounts
{
    public partial class Login
    {
        [Inject]
        private IAuthService AuthenticationService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private LoginModel loginModel = new();
        private bool showLoginErrorMessage;

        private const string style = "border-radius: 50px; width: 80%;";

        private async Task LogIn()
        {
            string userId = await AuthenticationService.LoginAsync(loginModel);
            if (string.IsNullOrEmpty(userId))
            {
                this.showLoginErrorMessage = true;
            }
            else
            {
                NavigationManager.NavigateTo("/");  //TODO: navigate to user's home page. Create a home component for each user type
            }

            loginModel = new();
        }
    }
}
