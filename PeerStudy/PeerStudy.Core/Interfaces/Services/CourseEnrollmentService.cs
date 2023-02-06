using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        private readonly IUnitOfWork unitOfWork;

        public CourseEnrollmentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<CourseEnrollmentRequest> CreateEnrollmentRequestAsync(Guid studentId, Guid courseId)
        {
            bool requestExists = await unitOfWork.CourseEnrollmentRequestsRepository.ExistsAsync(x => x.StudentId == studentId && x.CourseId == courseId);

            if (requestExists)
            {
                throw new DuplicateEntityException();
            }

            var enrollmentRequest = new CourseEnrollmentRequest
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = CourseEnrollmentRequestStatus.Pending
            };

            var insertedEnrollmentRequest = await unitOfWork.CourseEnrollmentRequestsRepository.AddAsync(enrollmentRequest);
            await unitOfWork.SaveChangesAsync();

            return insertedEnrollmentRequest;
        }
    }
}
