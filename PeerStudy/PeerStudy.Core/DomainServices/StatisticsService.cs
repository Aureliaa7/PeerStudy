using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUnitOfWork unitOfWork;

        public StatisticsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<StudyGroupStatisticsDataModel> GetStatisticsDataByGroupAsync(Guid studyGroupId, Guid courseId)
        {
            var studyGroup = await unitOfWork.StudyGroupRepository.GetFirstOrDefaultAsync(x => x.Id == studyGroupId,
                includeProperties: $"{nameof(StudyGroup.StudentStudyGroups)}")
                ?? throw new EntityNotFoundException($"Study group with id {studyGroupId} was not found!");

            int noLockedCourseUnits = await unitOfWork.CourseUnitsRepository.GetTotalRecordsAsync(x => x.CourseId == courseId && !x.IsAvailable);
            var studyGroupMembersIds = studyGroup.StudentStudyGroups.Select(x => x.StudentId)
                .ToList();

            var unlockedCourseUnits = await unitOfWork.UnlockedCourseUnitsRepository.GetAllAsync(x => studyGroupMembersIds.Contains(x.StudentId)
            && x.CourseUnit.CourseId == courseId);

            var students = await unitOfWork.UsersRepository.GetAllAsync(x => studyGroupMembersIds.Contains(x.Id));

            var assignmentsProgressStatistics = await GetAssignmentsProgressStatisticsAsync(students, studyGroupId, courseId, studyGroupMembersIds);

            return new StudyGroupStatisticsDataModel
            {
                StudyGroupId = studyGroupId,
                StudyGroupName = studyGroup.Name,
                AssignmentsStatistics = await GetAssignmentsStatisticsAsync(studyGroupId),
                AssignmentsProgress = assignmentsProgressStatistics.ToList(),
                NoLockedCourseUnits = noLockedCourseUnits,
                UnlockedCourseUnits = GetUnlockedCourseUnitsStatistics(unlockedCourseUnits, students, courseId)
            };
        }

        private async Task<StudyGroupAssignmentsStatisticsDataModel> GetAssignmentsStatisticsAsync(Guid studyGroupId)
        {
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId);

            return new StudyGroupAssignmentsStatisticsDataModel
            {
                CompletedOnTimeAssignments = assignments.Count(x => x.CompletedAt < x.Deadline),
                DoneLateAssignments = assignments.Count(x => x.CompletedAt > x.Deadline),
                MissingAssignments = assignments.Count(x => x.CompletedAt == null && x.Deadline < DateTime.UtcNow),
                ToDoAssignments = assignments.Count(x => x.Deadline > DateTime.UtcNow)
            };
        }

        private static List<StudentCourseUnitStatisticsDataModel> GetUnlockedCourseUnitsStatistics(
            IQueryable<UnlockedCourseUnit> unlockedCourseUnits,
            IQueryable<User> students,
            Guid courseId)
        {
            var unlockedCourseUnitsStatistics = (from s in students
                                                 join cu in unlockedCourseUnits.Where(x => x.CourseUnit.CourseId == courseId) on s.Id equals cu.StudentId into result
                                                 from cu in result.DefaultIfEmpty()
                                                 group cu by new { s.Id, s.FirstName, s.LastName } into g
                                                 select new StudentCourseUnitStatisticsDataModel
                                                 {
                                                     Id = g.Key.Id,
                                                     FullName = $"{g.Key.FirstName} {g.Key.LastName}",
                                                     UnlockedCourseUnits = g.Count(cu => cu != null)
                                                 })
                                                .ToList();

            return unlockedCourseUnitsStatistics;
        }

        private async Task<List<StudyGroupCourseUnitStatusModel>> GetAssignmentsProgressStatisticsAsync(
            IQueryable<User> students,
            Guid studyGroupId,
            Guid courseId,
            List<Guid> studyGroupMembersIds)
        {
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId && x.CompletedAt != null);
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.CourseId == courseId);
            var studentsAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => studyGroupMembersIds.Contains(x.StudentId));

            var assignmentsProgressStatistics = from assignment in assignments
                                                join studentAssignment in studentsAssignments on assignment.Id equals studentAssignment.AssignmentId
                                                join courseUnit in courseUnits on assignment.CourseUnitId equals courseUnit.Id
                                                join student in students on studentAssignment.StudentId equals student.Id
                                                where assignment.StudyGroupId == studyGroupId
                                                group new { assignment, studentAssignment, courseUnit, student } by new { assignment.CourseUnitId, courseUnit.Title, assignment.Id, assignmentTitle = assignment.Title } into g
                                                select new StudyGroupCourseUnitStatusModel
                                                {
                                                    CourseUnitId = g.Key.CourseUnitId,
                                                    CourseUnitTitle = g.Key.Title,
                                                    StudentAssignmentStatus = g.GroupBy(x => new { x.assignment.Id, x.assignment.Title, x.assignment.Points })
                                                                                .Select(sg => new StudyGroupAssignmentsModel
                                                                                {
                                                                                    AssignmentId = sg.Key.Id,
                                                                                    AssignmentTitle = sg.Key.Title,
                                                                                    NoMaxPoints = sg.Key.Points,
                                                                                    StudentAssignmentsStatus = sg.Select(sa => new StudentAssignmentStatusModel
                                                                                    {
                                                                                        Id = sa.studentAssignment.Student.Id,
                                                                                        NoEarnedPoints = sa.studentAssignment.Points ?? 0,
                                                                                        FullName = sa.student.FirstName + " " + sa.student.LastName,
                                                                                    }).ToList()
                                                                                }).ToList()
                                                };

            var result = assignmentsProgressStatistics
                .ToList()
                .GroupBy(x => new { x.CourseUnitId, x.CourseUnitTitle })
                .Select(x => new StudyGroupCourseUnitStatusModel
                {
                    CourseUnitId = x.Key.CourseUnitId,
                    CourseUnitTitle = x.Key.CourseUnitTitle,
                    StudentAssignmentStatus = x.SelectMany(x => x.StudentAssignmentStatus).ToList()
                })
                .ToList();

            return result;
        }
    }
}
