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

        Task<List<ExtendedAssignmentDetailsModel>> GetByCourseUnitIdAsync(Guid courseUnitId);

        Task<List<AssignmentDetailsModel>> GetAsync(Guid courseId, Guid studentId, AssignmentStatus status);

        Task DeleteAsync(Guid assignmentId);

        Task GradeAssignmentAsync(SaveGradeModel model);

        /// <summary>
        /// Called when a student cancels submission of work for an assignment.
        /// Resets the CompletedAt field from an Assignment entity
        /// </summary>
        /// <param name="assignmentId">The AssignmentId</param>
        /// <returns></returns>
        Task ResetSubmitDateAsync(Guid assignmentId);
    }
}
