using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.CourseEnrollments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService emailService;

        public CourseEnrollmentService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
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

                try
                {
                    await NotifyStudentsRegardingCourseEnrollmentRequestStatusAsync(
                         requests.Select(x => x.StudentId).ToList(),
                         requests.First().CourseId,
                         newStatus);
                }
                catch (Exception ex)
                {
                    //ToDo: log ex
                }

                return true;
            }
            catch (Exception ex)
            {
                // TODO: log the ex
                return false;
            }
        }

        private async Task NotifyStudentsRegardingCourseEnrollmentRequestStatusAsync(
            List<Guid> studentsIds,
            Guid courseId,
            CourseEnrollmentRequestStatus enrollmentsStatus)
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseId,
                includeProperties: nameof(Teacher)) ??
                throw new EntityNotFoundException();

            var studentsEmails = (await unitOfWork.UsersRepository.GetAllAsync(x => studentsIds.Contains(x.Id)
            && x.Role == Role.Student))
            .Select(x => x.Email)
            .ToList();

            if (!studentsEmails.Any())
            {
                //ToDo: log smth
                return;
            }

            CourseEnrollmentRequestStatusEmailModel emailModel = null;
            if (enrollmentsStatus == CourseEnrollmentRequestStatus.Approved)
            {
                emailModel = new ApprovedCourseEnrollmentRequestEmailModel
                {
                    CourseTitle = course.Title,
                    EmailType = EmailType.ApprovedCourseEnrollmentRequest,
                    TeacherName = $"{course.Teacher.FirstName} {course.Teacher.LastName}",
                    RecipientName = string.Empty,
                    To = studentsEmails
                };
            }
            else if (enrollmentsStatus == CourseEnrollmentRequestStatus.Rejected)
            {
                emailModel = new RejectedCourseEnrollmentRequestEmailModel
                {
                    CourseTitle = course.Title,
                    EmailType = EmailType.RejectedCourseEnrollmentRequest,
                    TeacherName = $"{course.Teacher.FirstName} {course.Teacher.LastName}",
                    RecipientName = string.Empty,
                    To = studentsEmails
                };
            }

            if (emailModel == null)
            {
                return;
            }

            await emailService.SendAsync(emailModel);
        }

        public async Task<CourseEnrollmentRequest> CreateEnrollmentRequestAsync(Guid studentId, Guid courseId)
        {
            bool studentIsAlreadyEnrolled = await unitOfWork.StudentCourseRepository.ExistsAsync(
                x => x.StudentId == studentId && x.CourseId == courseId);
            if (studentIsAlreadyEnrolled)
            {
                throw new DuplicateEntityException($"Student with id {studentId} is already enrolled in course with id {courseId}");
            }

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

            await NotifyTeacherAsync(courseId, studentId);
            
            return insertedEnrollmentRequest;
        }

        private async Task NotifyTeacherAsync(Guid courseId, Guid studentId)
        {
            try
            {
                var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseId,
                includeProperties: nameof(Teacher)) ?? throw new EntityNotFoundException();
                var student = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == studentId)
                    ?? throw new EntityNotFoundException();

                var emailModel = new CourseEnrollmentRequestEmailModel
                {
                    CourseTitle = course.Title,
                    EmailType = EmailType.SendCourseEnrollmentRequest,
                    RecipientName = $"{course.Teacher.FirstName} {course.Teacher.LastName}",
                    StudentName = $"{student.FirstName} {student.LastName}",
                    To = new List<string> { course.Teacher.Email }
                };

                await emailService.SendAsync(emailModel);
            }
            catch (Exception ex)
            {
                //TODO: log the ex
            }
        }

        public async Task<IList<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync(Guid teacherId, Guid courseId, CourseEnrollmentRequestStatus status)
        {
            bool courseExists = await unitOfWork.CoursesRepository.ExistsAsync(x => x.TeacherId == teacherId && x.Id == courseId);
            if (!courseExists)
            {
                throw new EntityNotFoundException();
            }

            var requests = (await unitOfWork.CourseEnrollmentRequestsRepository.GetAllAsync(x => x.CourseId == courseId &&
            x.Status == status, trackChanges: false)).Select(x => new CourseEnrollmentRequestDetailsModel 
            {
                Id = x.Id,
                CourseId = courseId,
                StudentId = x.StudentId,
                CourseTitle = x.Course.Title,
                StudentName = $"{x.Student.FirstName} {x.Student.LastName}",
                CreatedAt = x.CreatedAt
            })
            .ToList();

            return requests;
        }
        public async Task<CourseNoStudentsDetailsModel> GetCourseEnrollmentStatusAsync(Guid courseId)
        {
            var courseExists = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == courseId);
            if (!courseExists)
            {
                throw new EntityNotFoundException($"Course with id: {courseId} was not found!");
            }

            var enrolledStudentsStatus = (await unitOfWork.CoursesRepository.GetAllAsync(x => x.Id == courseId))
                .Select(x => new CourseNoStudentsDetailsModel
                {
                    NoEnrolledStudents = x.CourseEnrollments.Count(),
                    NoMaxStudents = x.NoStudents
                })
                .FirstOrDefault();

            return enrolledStudentsStatus;
        }
    }
}
