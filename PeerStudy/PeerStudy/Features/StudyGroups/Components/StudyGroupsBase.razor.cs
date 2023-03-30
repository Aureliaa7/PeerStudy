using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;

namespace PeerStudy.Features.StudyGroups.Components
{
    public abstract class StudyGroupsBase : PeerStudyComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }


        protected List<StudyGroupDetailsModel> studyGroups = new List<StudyGroupDetailsModel>();

        protected void HandleClickedStudyGroup(StudyGroupDetailsModel studyGroup)
        {
            NavigationManager.NavigateTo($"/{studyGroup.Id}/home");
        }
    }
}
