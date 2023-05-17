using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.StudentAssets;
using PeerStudy.Core.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AchievementService : IAchievementService
    {
        private readonly IUnitOfWork unitOfWork;

        public AchievementService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<StudentProfileModel> GetProgressByStudentIdAsync(Guid studentId, CourseStatus status)
        {
            var student = (Student)await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id ==
            studentId) ?? throw new EntityNotFoundException($"Student with id {studentId} was not found!");

            var studentProgress = new StudentProfileModel
            {
                Email = student.Email,
                Name = $"{student.FirstName} {student.LastName}",
                NoTotalPoints = student.NoTotalPoints,
            };

            studentProgress.CoursesProgress = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.StudentId == studentId && x.Course.Status == status))
                .Select(x => new StudentCourseProgressModel
                {
                    CourseId = x.CourseId,
                    CourseTitle = x.Course.Title,
                    TeacherName = $"{x.Course.Teacher.FirstName} {x.Course.Teacher.LastName}",
                    UnlockedCourseUnits = x.Student.UnlockedCourseUnits.Where(y => y.CourseUnit.CourseId == x.CourseId)
                    .Select(z => new UnlockedCourseUnitModel
                    {
                        CourseUnitTitle = z.CourseUnit.Title,
                        NoPaidPoints = z.CourseUnit.NoPointsToUnlock,
                        UnlockedAt = z.UnlockedAt
                    })
                    .ToList(),
                    CourseUnitsAssignmentsProgress = x.Student.Assignments
                    .GroupBy(y => y.Assignment.CourseUnit.Id)
                    .Where(g => g.Count() > 0 && g.Any(s => s.Assignment.CompletedAt != null && 
                    s.Assignment.CourseUnit.CourseId == x.CourseId &&
                    s.Assignment.CourseUnit.Course.Status == status))
                    .Select(b => new StudentCourseUnitAssignmentsModel
                    {
                        CourseUnitTitle = b.First().Assignment.CourseUnit.Title,
                        StudentAssignments = b.Where(z => z.Assignment.CompletedAt != null &&
                        z.Points != null).Select(c => new StudentAssignmentDetailsModel
                        {
                            AssignmentId = c.AssignmentId,
                            AssignmentTitle = c.Assignment.Title,
                            CompletedAt = c.Assignment.CompletedAt.Value,
                            NoEarnedPoints = c.Points.Value
                        })
                        .ToList()
                    })
                    .ToList()
                })
                .ToList();

            studentProgress.EarnedBadges = (await unitOfWork.StudentBadgesRepository.GetAllAsync(x => x.StudentId == studentId))
                .Select(x => new StudentBadgeDetailsModel
                {
                    Title = x.Badge.Title,
                    Base64Content = x.Badge.Base64Content,
                    Description = x.Badge.Description,
                    Points = x.Badge.Points,
                    Type = x.Badge.Type,
                    EarnedAt = x.EarnedAt
                })
                .ToList();

            return studentProgress;
        }
    }
}
