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

        Task<List<AssignmentDetailsModel>> GetByCourseAndStudentAsync(Guid courseId, Guid studentId, AssignmentStatus status);

        Task DeleteAsync(Guid assignmentId);

        Task GradeAssignmentAsync(SaveGradeModel model);

        /// <summary>
        /// Called when a student cancels submission of work for an assignment.
        /// Resets the CompletedAt field from an Assignment entity
        /// </summary>
        /// <param name="assignmentId">The AssignmentId</param>
        /// <returns></returns>
        Task ResetSubmitDateAsync(Guid assignmentId);

        Task<List<FlatAssignmentModel>> GetByStudentAsync(Guid studentId, AssignmentStatus status);

        /// <summary>
        /// Returns a list of assignments for a study group
        /// </summary>
        /// <param name="studyGroupId"></param>
        /// <returns></returns>
        Task<List<FlatAssignmentModel>> GetByStudyGroupAsync(Guid studyGroupId);

        /// <summary>
        /// Postpones a deadline
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="assignmentId"></param>
        /// <returns>The new deadline</returns>
        Task<DateTime> PostponeDeadlineAsync(Guid studentId, Guid assignmentId, Guid studyGroupId);

        /// <summary>
        /// Checks if a student can postpone a deadline 
        /// </summary>
        /// <param name="studyGroupId"></param>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        Task<bool> CanPostponeDeadlineAsync(Guid studyGroupId, Guid assignmentId);
    }
}
