using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudentAssets;
using System.Collections.Generic;

namespace PeerStudy.Features.Assets.Components.Badges.StudentBadgesComponent
{
    public partial class StudentBadges
    {
        [Parameter]
        public List<StudentBadgeDetailsModel> Badges { get; set; }
    }
}
