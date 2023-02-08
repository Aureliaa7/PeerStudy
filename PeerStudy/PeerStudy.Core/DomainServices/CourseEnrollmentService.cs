using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.CourseEnrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        private readonly IUnitOfWork unitOfWork;

        public CourseEnrollmentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> ChangeStatusAsync(List<CourseEnrollmentRequestDetailsModel> requests,
            CourseEnrollmentRequestStatus newStatus)
        {
            var ids = requests.Select(x => x.Id).ToList();

            var foundRequests = (await unitOfWork.CourseEnrollmentRequestsRepository.GetAllAsync())
                .Where(x => ids.Contains(x.Id))
                .ToList();
            var studentCourses = new List<StudentCourse>();

            foundRequests.ForEach(x => x.Status = newStatus);

            if (newStatus == CourseEnrollmentRequestStatus.Approved)
            {
                foundRequests.ForEach(x => studentCourses.Add(
                        new StudentCourse
                        {
                            CourseId = x.CourseId,
                            StudentId = x.StudentId
                        })
                );
            }

            try
            {
                await unitOfWork.StudentCourseRepository.AddRangeAsync(studentCourses);
                await unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // TODO: log the ex
                return false;
            }
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
                Status = CourseEnrollmentRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var insertedEnrollmentRequest = await unitOfWork.CourseEnrollmentRequestsRepository.AddAsync(enrollmentRequest);
            await unitOfWork.SaveChangesAsync();

            return insertedEnrollmentRequest;
        }

        public async Task<IList<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync(Guid teacherId, Guid courseId, CourseEnrollmentRequestStatus status)
        {
            bool courseExists = await unitOfWork.CoursesRepository.ExistsAsync(x => x.TeacherId == teacherId && x.Id == courseId);
            if (!courseExists)
            {
                throw new EntityNotFoundException();
            }

            var requests = (await unitOfWork.CourseEnrollmentRequestsRepository.GetAllAsync(x => x.CourseId == courseId &&
            x.Status == status, includeProperties: $"{nameof(Student)},{nameof(Course)}", trackChanges: false)).Select(x => new CourseEnrollmentRequestDetailsModel 
            {
                Id = x.Id,
                CourseId = courseId,
                StudentId = x.StudentId,
                CourseTitle = x.Course.Title,
                StudentName = $"{x.Student.FirstName} {x.Student.LastName}"
            })
            .ToList();

            return requests;
        }
    }
}
