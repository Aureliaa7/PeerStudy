using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.ActiveStudyGroupsComponent
{
    public partial class ActiveStudyGroups : StudentStudyGroupsBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override Task<List<StudentStudyGroupDetailsModel>> GetStudyGroupsDetailsAsync()
        {
            return StudyGroupService.GetByStudentIdAsync(currentUserId, CourseStatus.Active);
        }
    }
}
