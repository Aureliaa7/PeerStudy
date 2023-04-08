using Blazorise.Charts;
using PeerStudy.Core.Models.StudyGroups;
using PeerStudy.Features.StudyGroups.Components.StudyGroupCardBaseComponent;

namespace PeerStudy.Features.StudyGroups.Components.StudentStudyGroupCardComponent
{
    public partial class StudentStudyGroupCard : StudyGroupCardBase<StudentStudyGroupDetailsModel>
    {
        private ChartData<int> myWorkItemsChartData;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void CreateChartData()
        {
            base.CreateChartData();
            myWorkItemsChartData = ChartDataService.GetChartData(StudyGroupModel.MyWorkItemsStatus);
        }
    } 
}
