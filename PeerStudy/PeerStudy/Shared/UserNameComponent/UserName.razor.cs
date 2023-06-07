using Microsoft.AspNetCore.Components;

namespace PeerStudy.Shared.UserNameComponent
{
    public partial class UserName
    {
        [Parameter]
        public string FullName { get; set; }

        [Parameter]
        public string? ProfilePhotoName { get; set; }
    }
}
