using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.CourseEnrollments;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseEnrollmentService
    {
        Task<CourseEnrollmentRequest> CreateEnrollmentRequestAsync(Guid studentId, Guid courseId);

        Task<IList<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync(Guid teacherId, Guid courseId, CourseEnrollmentRequestStatus status);

        Task<bool> ChangeStatusAsync(List<CourseEnrollmentRequestDetailsModel> requests, CourseEnrollmentRequestStatus newStatus);

        Task<CourseNoStudentsDetailsModel> GetCourseEnrollmentStatusAsync(Guid courseId);
    }
}
