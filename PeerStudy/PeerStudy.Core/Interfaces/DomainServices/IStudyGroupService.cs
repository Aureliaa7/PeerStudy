using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.StudyGroups;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudyGroupService
    {
        Task CreateStudyGroupsAsync(Guid teacherId, Guid courseId, int noStudentsPerGroup);

        Task<List<StudyGroupDetailsModel>> GetByCourseIdAsync(Guid courseId);

        Task<List<StudentStudyGroupDetailsModel>> GetByStudentIdAsync(Guid studentId, CourseStatus courseStatus);

        Task<StudyGroupDetailsModel> GetAsync(Guid id);

        Task<List<UserModel>> GetStudentsByGroupIdAsync(Guid id);

        Task<bool> IsActiveAsync(Guid id);

        Task<Dictionary<Guid, string>> GetStudyGroupIdNamePairsAsync(Guid courseId);
    }
}
