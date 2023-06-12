using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Extensions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Emails;
using PeerStudy.Core.Models.StudentAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IStudentPointsService studentPointsService;
        private readonly IConfigurationService configurationService;
        private readonly IRewardingService rewardingService;
        private readonly IEmailService emailService;

        public AssignmentService(IUnitOfWork unitOfWork,
            IStudentPointsService studentPointsService,
            IConfigurationService configurationService,
            IRewardingService rewardingService,
            IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.studentPointsService = studentPointsService;
            this.configurationService = configurationService;
            this.rewardingService = rewardingService;
            this.emailService = emailService;
        }

        public async Task CreateAsync(CreateAssignmentModel model)
        {
            var courseUnitExists = await unitOfWork.CourseUnitsRepository.ExistsAsync(x => x.Id == model.CourseUnitId && 
            x.Course.TeacherId == model.TeacherId);
            if (!courseUnitExists)
            {
                throw new EntityNotFoundException($"Course unit with id {model.CourseUnitId} and teacher id {model.TeacherId} was not found!");
            }

            var addedAssignment = await unitOfWork.AssignmentsRepository.AddAsync(new Assignment
            {
                CreatedAt = DateTime.UtcNow,
                CourseUnitId = model.CourseUnitId,
                Deadline = model.DueDate,
                Description = model.Description,
                Title = model.Title,
                StudyGroupId = model.StudyGroupId,
                Points = model.Points
            });

            var studentIds = (await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.Id == model.StudyGroupId))
                .SelectMany(x => x.StudentStudyGroups
                    .Select(y => y.StudentId))
                .ToList();

            var studentAssignments = new List<StudentAssignment>();
            foreach (var studentId in studentIds)
            {
                studentAssignments.Add(new StudentAssignment
                {
                    Assignment = addedAssignment,
                    StudentId = studentId,
                    StudyGroupId = model.StudyGroupId
                });
            }
           
            await unitOfWork.StudentAssignmentsRepository.AddRangeAsync(studentAssignments);
            await unitOfWork.SaveChangesAsync();

            try
            {
                await NotifyStudentsAsync(model.StudyGroupId, model.CourseUnitId, model.Title, model.DueDate);
            }
            catch (Exception ex)
            {
                //TODO: log
            }
        }

        private async Task NotifyStudentsAsync(
            Guid studyGroupId,
            Guid courseUnitId,
            string assignmentTitle,
            DateTime assignmentDeadline)
        {
            var courseUnit = await unitOfWork.CourseUnitsRepository.GetFirstOrDefaultAsync(x => x.Id == courseUnitId,
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}") ?? throw new EntityNotFoundException();

            var studentsEmails = (await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x =>
            x.StudyGroupId == studyGroupId))
            .Select(x => x.Student.Email)
            .ToList();

            var emailModel = new NewAssignmentEmailModel
            {
                AssignmentTitle = assignmentTitle,
                CourseTitle = courseUnit.Course.Title,
                CourseUnitTitle = courseUnit.Title,
                Deadline = assignmentDeadline,
                EmailType = EmailType.NewAssignment,
                RecipientName = string.Empty,
                TeacherName = $"{courseUnit.Course.Teacher.FirstName} {courseUnit.Course.Teacher.LastName}",
                To = studentsEmails
            };

            await emailService.SendAsync(emailModel);
        }

        public async Task DeleteAsync(Guid assignmentId)
        {
            await CheckIfAssignmentExistsAsync(assignmentId);

            await unitOfWork.AssignmentsRepository.RemoveAsync(assignmentId);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AssignmentDetailsModel>> GetByCourseAndStudentAsync(Guid courseId, Guid studentId, AssignmentStatus status)
        {
            Expression<Func<Assignment, bool>> filter = GetFilterByAssignmentStatus(x => true, status);

            if (status == AssignmentStatus.Done)
            {
                var result = (await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.Assignment.CompletedAt != null && 
                x.StudentId == studentId && x.Assignment.CourseUnit.CourseId == courseId, trackChanges: false))
                    .Select(x => new AssignmentDetailsModel
                    {
                        Id = x.Assignment.Id,
                        Title = x.Assignment.Title,
                        Deadline = x.Assignment.Deadline,
                        CompletedAt = x.Assignment.CompletedAt,
                        CreatedAt = x.Assignment.CreatedAt,
                        Description = x.Assignment.Description,
                        Points = x.Assignment.Points,
                        StudentGroupId = x.Assignment.StudyGroupId
                    })
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                return result;
            }

            var studentStudyGroups = await unitOfWork.StudentStudyGroupRepository.GetAllAsync();
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(filter);
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync();
            var unlockedCourseUnits = await unitOfWork.UnlockedCourseUnitsRepository.GetAllAsync();
            var studyGroups = await unitOfWork.StudyGroupRepository.GetAllAsync();

            var availableAssignments = from a in assignments
                                       join cu in courseUnits on a.CourseUnitId equals cu.Id
                                       join sg in studyGroups on cu.CourseId equals sg.CourseId
                                       where sg.Id == a.StudyGroupId &&
                                             cu.CourseId == courseId && (cu.IsAvailable || 
                                             studentStudyGroups.Any(ssg =>
                                                ssg.StudentId == studentId &&
                                                ssg.StudyGroupId == a.StudyGroupId &&
                                                unlockedCourseUnits.Any(ucu =>
                                                    ucu.StudentId == ssg.StudentId &&
                                                    ucu.CourseUnitId == cu.Id)))
                                       select a;

            var foundAssignments = availableAssignments
            .Select(x => new AssignmentDetailsModel
            {
                StudentGroupId = x.StudyGroupId,
                Deadline = x.Deadline,
                Description = x.Description,
                Id = x.Id,
                Title = x.Title,
                Points = x.Points,
                CreatedAt = x.CreatedAt
            })
            .OrderBy(x => x.CreatedAt)
            .ToList();

            return foundAssignments;
        }

        private static Expression<Func<Assignment, bool>> GetFilterByAssignmentStatus(Expression<Func<Assignment, bool>> filter, AssignmentStatus status)
        {
            if (status == AssignmentStatus.Done)
            {
                return filter.And(x => x.CompletedAt != null);
            }
            else if (status == AssignmentStatus.Upcoming)
            {
                return filter.And(x => x.CompletedAt == null &&
                    x.Deadline > DateTime.UtcNow);
            }
            else if (status == AssignmentStatus.Missing)
            {
                return filter.And(x => x.CompletedAt == null &&
                   x.Deadline < DateTime.UtcNow);
            }
            else
            {
                return filter;
            }
        }

        public async Task<List<ExtendedAssignmentDetailsModel>> GetByCourseUnitIdAsync(Guid courseUnitId)
        {
            var assignments = (await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.CourseUnitId == courseUnitId,
                trackChanges: false))
                .Select(x => new ExtendedAssignmentDetailsModel
                {
                    Deadline = x.Deadline,
                    Id = x.Id,
                    Title = x.Title,
                    StudentGroupId = x.StudyGroupId,
                    Points = x.Points,
                    CompletedAt = x.CompletedAt,
                    Students = x.StudentAssignments.Select(y => new GradeAssignmentModel
                    {
                        StudentId = y.StudentId,
                        StudentName = $"{y.Student.FirstName} {y.Student.LastName}",
                        //Note: fix for DataGrid component. It does not allow null values
                        Points = y.Points ?? 0,
                        HasBeenGraded = y.Points != null
                    })
                    .ToList(),
                    StudyGroupName = x.StudyGroup.Name,
                    CourseUnitTitle = x.CourseUnit.Title
                })
                .ToList();

            return assignments;
        }

        public async Task GradeAssignmentAsync(SaveGradeModel model)
        {
            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x => x.AssignmentId ==
            model.AssignmentId && x.StudentId == model.StudentId, includeProperties: nameof(Student)) ?? throw new EntityNotFoundException($"StudentAssignment entity with studentId {model.StudentId} and assignmentId {model.AssignmentId} " +
                    $"was not found!");
           
            studentAssignment.Points = model.Points;
            await unitOfWork.StudentAssignmentsRepository.UpdateAsync(studentAssignment);
            await unitOfWork.SaveChangesAsync();

            await SavePointsAsync(model.StudentId, model.Points);

            await rewardingService.UpdateBadgesForCourseAsync(model.CourseId);

            await NotifyStudentRegardingGradedAssignmentAsync(studentAssignment.Student, model.AssignmentId, model.Points);
        }

        private async Task NotifyStudentRegardingGradedAssignmentAsync(Student student, Guid assignmentId, int noPoints)
        {
            try
            {
                var assignment = await unitOfWork.AssignmentsRepository.GetFirstOrDefaultAsync(x =>
                    x.Id == assignmentId, includeProperties: $"{nameof(CourseUnit)}.{nameof(Course)}.{nameof(Teacher)}")
                ?? throw new EntityNotFoundException();

                var emailModel = new GradedAssignmentEmailModel
                {
                    AssignmentTitle = assignment.Title,
                    CourseTitle = assignment.CourseUnit.Course.Title,
                    CourseUnitTitle = assignment.CourseUnit.Title,
                    EmailType = EmailType.GradedAssignment,
                    NoPoints = noPoints,
                    RecipientName = $"{student.FirstName} {student.LastName}",
                    TeacherName = $"{assignment.CourseUnit.Course.Teacher.FirstName} {assignment.CourseUnit.Course.Teacher.LastName}",
                    To = new List<string> { student.Email }
                };

                await emailService.SendAsync(emailModel);
            }
            catch (Exception ex)
            {
                //ToDo: log
            }
        }

        private async Task SavePointsAsync(Guid studentId, int noPoints)
        {
            await studentPointsService.AddAsync(new SaveStudentPointsModel
            {
                NoPoints = noPoints,
                StudentId = studentId
            });
        }

        private async Task CheckIfAssignmentExistsAsync(Guid id)
        {
            bool assignmentExists = await unitOfWork.AssignmentsRepository.ExistsAsync(x => x.Id == id);

            if (!assignmentExists)
            {
                throw new EntityNotFoundException($"Assignment with id {id} was not found!");
            }
        }

        public async Task ResetSubmitDateAsync(Guid assignmentId)
        {
            var assignment = await unitOfWork.AssignmentsRepository.GetFirstOrDefaultAsync(x => x.Id == assignmentId) ?? throw new EntityNotFoundException($"Assignment with id {assignmentId} was not found!");
            
            assignment.CompletedAt = null;
            await unitOfWork.AssignmentsRepository.UpdateAsync(assignment);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FlatAssignmentModel>> GetByStudentAsync(Guid studentId, AssignmentStatus status)
        {
            Expression<Func<Assignment, bool>> filter = GetFilterByAssignmentStatus(x => true, status);

            if (status == AssignmentStatus.Done)
            {
                var result = (await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.Assignment.CompletedAt != null && x.StudentId == studentId, trackChanges: false))
                    .OrderBy(x => x.Assignment.CreatedAt)
                    .Select(x => new FlatAssignmentModel
                    {
                        Id = x.Assignment.Id,
                        Title = x.Assignment.Title,
                        CourseTitle = x.Assignment.CourseUnit.Course.Title,
                        Deadline = x.Assignment.Deadline,
                        CourseId = x.Assignment.CourseUnit.CourseId,
                        StudyGroupId = x.StudyGroupId,
                        CourseUnitTitle = x.Assignment.CourseUnit.Title
                    })
                    .ToList();

                return result;
            }

            var studentCourses = await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.StudentId == studentId);
            var studentStudyGroups = await unitOfWork.StudentStudyGroupRepository.GetAllAsync();
            var assignments = await unitOfWork.AssignmentsRepository.GetAllAsync(filter);
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync();
            var unlockedCourseUnits = await unitOfWork.UnlockedCourseUnitsRepository.GetAllAsync();
            var studyGroups = await unitOfWork.StudyGroupRepository.GetAllAsync();

            var availableAssignments =
                from sc in studentCourses
                join cu in courseUnits on sc.CourseId equals cu.CourseId
                join sg in studyGroups on cu.CourseId equals sg.CourseId
                join a in assignments on cu.Id equals a.CourseUnitId
                where sg.Id == a.StudyGroupId && (cu.IsAvailable ||
                  studentStudyGroups.Any(ssg =>
                     ssg.StudentId == studentId &&
                     ssg.StudyGroupId == a.StudyGroupId &&
                     unlockedCourseUnits.Any(ucu =>
                         ucu.StudentId == ssg.StudentId &&
                         ucu.CourseUnitId == cu.Id)))
                select a;


            var foundAssignments = availableAssignments
                .OrderBy(x => x.CreatedAt)
                .Select(x => new FlatAssignmentModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    CourseTitle = x.CourseUnit.Course.Title,
                    Deadline = x.Deadline,
                    CourseId = x.CourseUnit.CourseId,
                    StudyGroupId = x.StudyGroupId,
                    CourseUnitTitle = x.CourseUnit.Title
                })
                .ToList();

             return foundAssignments;
        }

        public async Task<List<FlatAssignmentModel>> GetByStudyGroupAsync(Guid studyGroupId)
        {
            var assignments = (await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId, trackChanges: false))
            .Select(x => new FlatAssignmentModel
            {
                Id = x.Id,
                Title = x.Title,
                CourseTitle = x.CourseUnit.Course.Title,
                Deadline = x.Deadline,
                CourseId = x.CourseUnit.CourseId,
                StudyGroupId = x.StudyGroupId,
                CompletedAt = x.CompletedAt,
                CourseUnitTitle = x.CourseUnit.Title
            })
            .ToList();

            return assignments;
        }

        public async Task<DateTime> PostponeDeadlineAsync(Guid studentId, Guid assignmentId, Guid studyGroupId)
        {
            var assignment = await GetByIdAsync(assignmentId);
            var studyGroupMembers = await GetStudyGroupMembersAsync(studyGroupId);

            if (!await CanPostponeDeadlineAsync(studyGroupId, assignment, studyGroupMembers))
            {
                throw new PreconditionFailedException("The study group reached the max number of postponed assignments" +
                    "or does not have enough points...");
            }
            var noStudentsInStudyGroup = (await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId)).Count();

            var noPointsPaidByAMember = assignment.Points / 2;

            var postponedAssignment = new PostponedAssignment
            {
                AssignmentId = assignmentId,
                CreatedAt = DateTime.UtcNow,
                NoTotalPaidPoints = noStudentsInStudyGroup * noPointsPaidByAMember,
                StudentId = studentId,
                StudyGroupId = studyGroupId
            };
            await unitOfWork.PostponedAssignmentsRepository.AddAsync(postponedAssignment);

            assignment.Deadline = assignment.Deadline?.AddDays(configurationService.NoDaysToPostponeDeadline);
            await unitOfWork.AssignmentsRepository.UpdateAsync(assignment);
            foreach (var student in studyGroupMembers)
            {
                student.NoTotalPoints -= assignment.Points;
                await unitOfWork.UsersRepository.UpdateAsync(student);
            }

            await unitOfWork.SaveChangesAsync();

            return assignment.Deadline.Value;
        }

        public async Task<bool> CanPostponeDeadlineAsync(Guid studyGroupId, Guid assignmentId)
        {
            var assignment = await GetByIdAsync(assignmentId);
            if (assignment.CompletedAt == null && assignment.Deadline < DateTime.UtcNow)
            {
                return false;
            }

            var studyGroupMembers = await GetStudyGroupMembersAsync(studyGroupId);

            return await CanPostponeDeadlineAsync(studyGroupId, assignment, studyGroupMembers);
        }

        private async Task<Assignment> GetByIdAsync(Guid assignmentId)
        {
            return await unitOfWork.AssignmentsRepository.GetByIdAsync(assignmentId) ??
             throw new EntityNotFoundException($"The assignment with id {assignmentId} does not exist!");
        }

        private async Task<bool> CanPostponeDeadlineAsync(Guid studyGroupId, Assignment assignment, List<Student> students)
        {
            var noPostponedDeadlines = (await unitOfWork.PostponedAssignmentsRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId))
                .Count();

            bool canPostponeDeadline = noPostponedDeadlines < configurationService.MaxPostponedDeadlinesPerStudyGroup;
            bool studentsHaveNecessaryPoints = StudyGroupCanPostponeDeadline(students, assignment);

            return canPostponeDeadline && studentsHaveNecessaryPoints;
        }

        private bool StudyGroupCanPostponeDeadline(List<Student> studyGroupMembers, Assignment assignment)
        {
            bool canPostponeDeadline = !(studyGroupMembers.Any(x => x.NoTotalPoints < assignment.Points / 2));

            return canPostponeDeadline;
        }

        private async Task<List<Student>> GetStudyGroupMembersAsync(Guid studyGroupId)
        {
            var students = (await unitOfWork.StudentStudyGroupRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId))
               .Select(x => x.Student)
               .ToList();

            return students;
        }
    }
}
