using Microsoft.AspNetCore.Components;

namespace PeerStudy.Features.Assets.Components.Leaderboards.LeaderboardItemComponent
{
    public partial class LeaderboardItem
    {
        [Parameter]
        public int Rank { get; set; }

        [Parameter]
        public string StudentName { get; set; }

        [Parameter]
        public string ProfilePhotoName { get; set; }

        [Parameter]
        public int EarnedPoints { get; set; }
    }
}
