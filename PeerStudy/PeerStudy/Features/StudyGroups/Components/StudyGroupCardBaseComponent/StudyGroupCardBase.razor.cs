using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using PeerStudy.Services.Interfaces;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupCardBaseComponent
{
    public partial class StudyGroupCardBase<T> : ComponentBase where T: StudyGroupDetailsModel
    {
        [Inject]
        protected IChartDataService ChartDataService { get; set; }

        [Parameter]
        public T StudyGroupModel { get; set; }

        [Parameter]
        public EventCallback<T> OnClicked { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected ChartData<int> allWorkItemsChartData;

        private const string styles = "width: 90%; height: 95%; padding: 5px;";

        protected override void OnInitialized()
        {
            CreateChartData();
        }

        protected async Task ClickHandler()
        {
            await OnClicked.InvokeAsync(StudyGroupModel);
        }

        protected virtual void CreateChartData()
        {
            allWorkItemsChartData = ChartDataService.GetChartData(StudyGroupModel.AllWorkItemsStatus);
        }
    }
}
