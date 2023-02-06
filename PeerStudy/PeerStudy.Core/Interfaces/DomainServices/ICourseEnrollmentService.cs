using PeerStudy.Core.DomainEntities;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseEnrollmentService
    {
        Task<CourseEnrollmentRequest> CreateEnrollmentRequestAsync(Guid studentId, Guid courseId); 
    }
}
