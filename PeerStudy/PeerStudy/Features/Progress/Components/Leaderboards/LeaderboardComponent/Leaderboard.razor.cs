using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.ProgressModels;
using System.Collections.Generic;

namespace PeerStudy.Features.Progress.Components.Leaderboards.LeaderboardComponent
{
    public partial class Leaderboard
    {
        [Parameter]
        public List<StudentProgressModel> StudentsProgress { get; set; }
    }
}
