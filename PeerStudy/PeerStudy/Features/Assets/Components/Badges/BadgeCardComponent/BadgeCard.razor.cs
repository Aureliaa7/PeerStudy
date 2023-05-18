using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudentAssets;

namespace PeerStudy.Features.Assets.Components.Badges.BadgeCardComponent
{
    public partial class BadgeCard
    {
        [Parameter]
        public StudentBadgeDetailsModel Badge { get; set; }
    }
}
