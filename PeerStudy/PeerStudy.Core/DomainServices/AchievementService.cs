using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.ProgressModels;
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

        public async Task<List<StudyGroupLeaderboardModel>> GetLeaderboardDataForStudyGroupsAsync(Guid courseId, Guid teacherId)
        {
            await CheckIfCourseExistsAsync(courseId, teacherId);

            var studyGroups = await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.CourseId == courseId);
            var studentStudyGroups = await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var studentAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);

            var result = from studyGroup in studyGroups
                                     join assignment in assignments on studyGroup.Id equals assignment.StudyGroupId
                                     join studentStudyGroup in studentStudyGroups on studyGroup.Id equals studentStudyGroup.StudyGroupId
                                     join studentAssignment in studentAssignments on new { AssignmentID = assignment.Id, StudentID = studentStudyGroup.StudentId } equals new { AssignmentID = studentAssignment.AssignmentId, StudentID = studentAssignment.StudentId }
                                     group studentAssignment by new { studyGroup.Id, studyGroup.Name, studentStudyGroup.StudentId, studentStudyGroup.Student.FirstName, studentStudyGroup.Student.LastName, studentAssignment.Student.ProfilePhotoName } into g
                                     select new 
                                     {
                                         StudyGroupName = g.Key.Name,
                                         StudyGroupId = g.Key.Id,
                                         g.Key.StudentId,
                                         StudentName = g.Key.FirstName + " " + g.Key.LastName,
                                         g.Key.ProfilePhotoName,
                                         TotalPoints = g.Sum(sa => sa.Points)
                                     };

            var studyGroupsProgress = result
                .ToList()
                .GroupBy(r => new { r.StudyGroupId, r.StudyGroupName })
                .Select(g => new StudyGroupLeaderboardModel
                {
                    StudyGroupName = g.Key.StudyGroupName,
                    StudentsProgress = g.OrderByDescending(r => r.TotalPoints)
                                .Select((r, rank) => new StudentProgressModel
                                {
                                    StudentId = r.StudentId,
                                    Name = r.StudentName,
                                    NoPoints = r.TotalPoints ?? 0,
                                    ProfilePhotoName = r.ProfilePhotoName
                                })
                                .ToList()
                })
                .OrderBy(x => x.StudyGroupName)
                .ToList();

            foreach (var studyGroupProgress in  studyGroupsProgress)
            {
                SetRank(studyGroupProgress.StudentsProgress);
            }

            return studyGroupsProgress;
        }

        public async Task<List<StudentProgressModel>> GetLeaderboardDataByCourseAsync(Guid courseId, Guid teacherId)
        {
            await CheckIfCourseExistsAsync(courseId, teacherId);

            var studyGroups = await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.CourseId == courseId);
            var studentStudyGroups = await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var studentAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);

            var result = from studyGroup in studyGroups
                         join assignment in assignments on studyGroup.Id equals assignment.StudyGroupId
                         join studentStudyGroup in studentStudyGroups on studyGroup.Id equals studentStudyGroup.StudyGroupId
                         join studentAssignment in studentAssignments on new { AssignmentID = assignment.Id, StudentID = studentStudyGroup.StudentId } equals new { AssignmentID = studentAssignment.AssignmentId, StudentID = studentAssignment.StudentId }
                         group studentAssignment by new { studyGroup.Id, studyGroup.Name, studentStudyGroup.StudentId, studentStudyGroup.Student.FirstName, studentStudyGroup.Student.LastName, studentAssignment.Student.ProfilePhotoName } into g
                         select new
                         {
                             StudyGroupName = g.Key.Name,
                             StudyGroupId = g.Key.Id,
                             g.Key.StudentId,
                             StudentName = g.Key.FirstName + " " + g.Key.LastName,
                             g.Key.ProfilePhotoName,
                             TotalPoints = g.Sum(sa => sa.Points)
                         };

            var studentsProgress = result
                .ToList()
                .OrderByDescending(x => x.TotalPoints)
                .Select((x, rank) => new StudentProgressModel
                {    
                    StudentId = x.StudentId,
                    Name = x.StudentName,
                    NoPoints = x.TotalPoints ?? 0,
                    ProfilePhotoName = x.ProfilePhotoName                
                })
                .ToList();

            SetRank(studentsProgress);

            return studentsProgress;
        }

        private static void SetRank(List<StudentProgressModel> studentsProgress)
        {
            int rank = 1;
            int previousPoints = studentsProgress.First().NoPoints;

            foreach (var progress in studentsProgress)
            {
                if (progress.NoPoints < previousPoints)
                {
                    rank++;
                    progress.Rank = rank;
                    previousPoints = progress.NoPoints;
                }
                else
                {
                    progress.Rank = rank;
                }
            }
        }

        private async Task CheckIfCourseExistsAsync(Guid courseId, Guid teacherId)
        {
            bool courseExists = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == courseId && x.TeacherId == teacherId);
            if (!courseExists)
            {
                throw new EntityNotFoundException($"Course with id {courseId} does not exist!");
            }
        }

        public async Task<List<CourseUnitLeaderboardModel>> GetCourseUnitsLeaderboardDataAsync(Guid courseId, Guid teacherId)
        {
            await CheckIfCourseExistsAsync(courseId, teacherId);

            var studyGroups = await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.CourseId == courseId);
            var studentStudyGroups = await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var studentAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.StudyGroup.CourseId == courseId);
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.CourseId == courseId);

            var result = from studyGroup in studyGroups
                         join assignment in assignments on studyGroup.Id equals assignment.StudyGroupId
                         join courseUnit in courseUnits on assignment.CourseUnitId equals courseUnit.Id
                         join studentStudyGroup in studentStudyGroups on studyGroup.Id equals studentStudyGroup.StudyGroupId
                         join studentAssignment in studentAssignments on new { AssignmentID = assignment.Id, StudentID = studentStudyGroup.StudentId } equals new { AssignmentID = studentAssignment.AssignmentId, StudentID = studentAssignment.StudentId }
                         group studentAssignment by new { courseUnit.Id, courseUnit.Title, studentStudyGroup.Student.FirstName, studentStudyGroup.Student.LastName, studentAssignment.Student.ProfilePhotoName, studentStudyGroup.StudentId } into g
                         select new
                         {
                             CourseUnitTitle = g.Key.Title,
                             StudentId = g.Key.StudentId,
                             StudentName = g.Key.FirstName + " " + g.Key.LastName,
                             g.Key.ProfilePhotoName,
                             TotalPoints = g.Sum(sa => sa.Points)
                         };

            var courseUnitsProgress = result
                .ToList()
                .GroupBy(r => new { r.CourseUnitTitle})
                .Select(g => new CourseUnitLeaderboardModel
                {
                    CourseUnitTitle = g.Key.CourseUnitTitle,
                    StudentProgressModels = g.OrderByDescending(r => r.TotalPoints)
                                .Select((r, rank) => new StudentProgressModel
                                {
                                    StudentId = r.StudentId,
                                    Name = r.StudentName,
                                    NoPoints = r.TotalPoints ?? 0,
                                    ProfilePhotoName = r.ProfilePhotoName
                                })
                                .ToList()
                })
                .OrderBy(x => x.CourseUnitTitle)
                .ToList();

            foreach (var courseUnit in courseUnitsProgress)
            {
                SetRank(courseUnit.StudentProgressModels);
            }

            return courseUnitsProgress;
        }

        public async Task<ExtendedStudentCourseProgressModel> GetProgressByCourseAndStudentAsync(Guid courseId, Guid studentId, Guid teacherId)
        {
            await CheckIfCourseExistsAsync(courseId, teacherId);
            var student = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == studentId) ??
                throw new EntityNotFoundException($"The student with id {studentId} was not found!");

            var courseProgress = await GetCourseProgressByStudentAsync(studentId, courseId);
            var progressModel = new ExtendedStudentCourseProgressModel
            {
                CourseUnitsAssignmentsProgress = courseProgress.CourseUnitsAssignmentsProgress,
                UnlockedCourseUnits = courseProgress.UnlockedCourseUnits,
                CourseId = courseId,
                CourseTitle = courseProgress.CourseTitle,
                TeacherName = courseProgress.TeacherName,
                Name = $"{student.FirstName} {student.LastName}",
                Email = student.Email,
                AllBadges = await GetBadgesByStudentAsync(studentId)
            };

            return progressModel;
        }

        private async Task<StudentCourseProgressModel> GetCourseProgressByStudentAsync(Guid studentId, Guid courseId)
        {
            var courseProgress = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.StudentId == studentId && x.Course.Id == courseId))
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
                    .Where(g => g.Count() > 0 && g.Any(
                        s => s.Assignment.CompletedAt != null && s.Assignment.CourseUnit.CourseId == courseId))
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
                .FirstOrDefault();

            return courseProgress;
        }
    }
}
