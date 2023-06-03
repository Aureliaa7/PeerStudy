using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.StudyGroups;

namespace PeerStudy.Features.StudyGroups.Components
{
    public abstract class StudyGroupsBase : PeerStudyComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IStudyGroupService StudyGroupService { get; set; }

        protected void HandleClickedStudyGroup(StudyGroupDetailsModel studyGroup)
        {
            NavigationManager.NavigateTo($"/{studyGroup.Id}/home");
        }
    }
}
