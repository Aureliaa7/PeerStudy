using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.ArchivedStudyGroupsComponent
{
    public partial class ArchivedStudyGroups : StudentStudyGroupsBase
    {
        protected override Task<List<StudentStudyGroupDetailsModel>> GetStudyGroupsDetailsAsync()
        {
            return StudyGroupService.GetByStudentIdAsync(currentUserId, CourseStatus.Archived);
        }
    }
}
