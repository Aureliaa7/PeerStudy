using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupCardComponent
{
    public partial class StudyGroupCard
    {
        [Parameter]
        public StudyGroupDetailsModel StudyGroupModel { get; set; } 

        [Parameter]
        public EventCallback<StudyGroupDetailsModel> OnClicked { get; set; }

        private const string styles = "width: 90%; height: 95%; padding: 5px;";
        private ChartData<int> chartData;

        protected override void OnInitialized()
        {
            CreateChartData();
        }

        private async Task ClickHandler()
        {
            await OnClicked.InvokeAsync(StudyGroupModel);
        }


        //TODO: move background-colors; this component should be not be aware of them
        private void CreateChartData()
        {
            chartData = new ChartData<int>
            {
                Datasets = new List<ChartDataset<int>>
                {
                    new ChartDataset<int>
                    {
                        Data = StudyGroupModel.WorkItemsStatus
                                .OrderBy(x => x.Key)
                                .Select(x => x.Value)
                                .ToList(),
                        BackgroundColor = new List<string> { "rgba(75, 192, 192, 0.6)" , "rgba(54, 162, 235, 0.6)" , "rgba(255, 99, 132, 0.6)" }
                    }
                },
                Labels = StudyGroupModel.WorkItemsStatus.OrderBy(x => x.Key).Select(x => (object) x.Key.ToString()).ToList()
            };
        }
    }
}
