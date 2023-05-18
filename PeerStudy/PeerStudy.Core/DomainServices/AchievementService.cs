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
using System.Collections.Generic;
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

            studentProgress.CoursesProgress = (await GetCoursesProgressByStudentAsync(studentId, status))
                .OrderBy(x => x.CourseTitle)
                .ToList();

            studentProgress.EarnedBadges = await GetBadgesByStudentAsync(studentId);
            studentProgress.CourseRankings = (await GetStudentCoursesRankingAsync(studentId, status))
                .OrderBy(x => x.CourseTitle)
                .ToList();

            return studentProgress;
        }

        private async Task<List<StudentCourseProgressModel>> GetCoursesProgressByStudentAsync(Guid studentId, CourseStatus courseStatus)
        {
            var coursesProgress = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.StudentId == studentId && x.Course.Status == courseStatus))
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
                    s.Assignment.CourseUnit.Course.Status == courseStatus))
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
                    .OrderBy(x => x.CourseUnitTitle)
                    .ToList()
                })
                .ToList();

            return coursesProgress;
        }

        private async Task<List<StudentBadgeDetailsModel>> GetBadgesByStudentAsync(Guid studentId)
        {
            var earnedBadges = (await unitOfWork.StudentBadgesRepository.GetAllAsync(x => x.StudentId == studentId))
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

            return earnedBadges;
        }

        private async Task<List<CourseRankingModel>> GetStudentCoursesRankingAsync(Guid studentId, CourseStatus status)
        {
            var courseRankingGroups = await GetCourseRankingGroupsAsync(status);
            
            var studentRankingsPerCourses = new List<CourseRankingModel>();
            foreach (var group in courseRankingGroups)
            {
                int studentRank = 1;

                foreach (var ranking in group)
                {
                    ranking.Rank = studentRank;
                    studentRank++;
                    
                    if (ranking.StudentId == studentId)
                    {
                        studentRankingsPerCourses.Add(ranking);
                    }
                }
            }

            return studentRankingsPerCourses;
        }

        private async Task<List<IGrouping<string, CourseRankingModel>>> GetCourseRankingGroupsAsync(CourseStatus status)
        {
            var courses = await unitOfWork.CoursesRepository.GetAllAsync(x => x.Status == status);
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.Course.Status == status);
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.CourseUnit.Course.Status == status);
            var studentCourses = await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.Course.Status == status);
            var studentAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.Assignment.CourseUnit.Course.Status == status);

            var courseRankingGroups = (from course in courses
                                       join courseUnit in courseUnits on course.Id equals courseUnit.CourseId
                                       join assignment in assignments on courseUnit.Id equals assignment.CourseUnitId
                                       join studentCourse in studentCourses on course.Id equals studentCourse.CourseId
                                       join studentAssignment in studentAssignments on new { StudentId = studentCourse.StudentId, AssignmentId = assignment.Id } equals new { StudentId = studentAssignment.StudentId, AssignmentId = studentAssignment.AssignmentId } into sa
                                       from studentAssignment in sa.DefaultIfEmpty()
                                       group studentAssignment by new { course.Title, studentCourse.Student.FirstName, studentCourse.Student.LastName, studentCourse.StudentId, studentCourse.Student.ProfilePhotoName} into g
                                       orderby g.Key.Title
                                       select new CourseRankingModel
                                       {
                                           CourseTitle = g.Key.Title,
                                           StudentName = g.Key.FirstName + " " + g.Key.LastName,
                                           EarnedPoints = g.Sum(sa => sa.Points) ?? 0,
                                           StudentId = g.Key.StudentId,
                                           ProfilePhotoName = g.Key.ProfilePhotoName,
                                       })
                                .ToList()
                                .OrderByDescending(x => x.EarnedPoints)
                                .GroupBy(x => x.CourseTitle)
                                .ToList();

            return courseRankingGroups;
        }
    }
}
