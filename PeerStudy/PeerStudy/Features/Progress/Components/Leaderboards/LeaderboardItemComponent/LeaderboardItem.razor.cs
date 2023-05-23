using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Progress.Components.Leaderboards.LeaderboardItemComponent
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

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public bool CanBeClicked { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClick { get; set; }


        private async Task HandleClickEvent()
        {
            if (CanBeClicked)
            {
                await OnClick.InvokeAsync(StudentId);
            }
        }
    }
}
