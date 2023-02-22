using PeerStudy.Core.Models.StudyGroups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudyGroupService
    {
        Task CreateStudyGroupsAsync(Guid teacherId, Guid courseId, int noStudentsPerGroup);

        Task<List<StudyGroupDetailsModel>> GetByCourseIdAsync(Guid courseId);

        Task<List<StudyGroupDetailsModel>> GetByStudentIdAsync(Guid studentId);
    }
}
