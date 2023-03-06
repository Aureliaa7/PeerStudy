using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAssignmentService
    {
        Task CreateAsync(CreateAssignmentModel model);

        Task<List<ExtendedAssignmentDetailsModel>> GetByCourseIdAsync(Guid courseId);

        Task<List<AssignmentDetailsModel>> GetAsync(Guid courseId, Guid studentId, AssignmentStatus status);

        Task DeleteAsync(Guid assignmentId);

        Task GradeAssignmentAsync(SaveGradeModel model);
    }
}
