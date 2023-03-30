using Blazorise.Charts;
using Microsoft.AspNetCore.Components;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupWorkItemsOverviewComponent
{
    public partial class StudyGroupWorkItemsOverview
    {
        [Parameter]
        public ChartData<int> Data { get; set; }
    }
}
