using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupsListComponent
{
    public partial class StudyGroupsList
    {
        [Parameter]
        public List<StudyGroupDetailsModel> StudyGroups { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<StudyGroupDetailsModel> OnClicked { get; set; }

        private async Task HandleClick(StudyGroupDetailsModel studyGroup)
        {
            await OnClicked.InvokeAsync(studyGroup);
        } 
    }
}
