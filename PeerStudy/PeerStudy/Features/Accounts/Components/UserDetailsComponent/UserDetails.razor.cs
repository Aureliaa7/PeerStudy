using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Accounts;
using System.Threading.Tasks;

namespace PeerStudy.Features.Accounts.Components.UserDetailsComponent
{
    public partial class UserDetails : PeerStudyComponentBase
    {
        [Inject]
        private IAccountService AccountService { get; set; }

        private UserDetailsModel userDetails;

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            userDetails = await AccountService.GetUserDetailsAsync(currentUserId);
        }
    }
}
