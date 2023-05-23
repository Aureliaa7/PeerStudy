using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Progress.Components.Leaderboards.LeaderboardComponent
{
    public partial class Leaderboard
    {
        [Parameter]
        public List<StudentProgressModel> StudentsProgress { get; set; }

        [Parameter]
        public bool ItemsCanBeClicked { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClick { get; set; }
        
        private async Task OnItemClicked(Guid studentId)
        {
            if (ItemsCanBeClicked)
            {
                await OnClick.InvokeAsync(studentId);
            }
        }
    }
}
