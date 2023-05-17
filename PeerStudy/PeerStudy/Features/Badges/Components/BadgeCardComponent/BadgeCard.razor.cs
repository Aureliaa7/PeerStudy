using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudentAssets;

namespace PeerStudy.Features.Badges.Components.BadgeCardComponent
{
    public partial class BadgeCard
    {
        [Parameter]
        public StudentBadgeDetailsModel Badge { get; set; }
    }
}
