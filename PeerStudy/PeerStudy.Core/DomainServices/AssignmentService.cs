using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Assignments;
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

        public AssignmentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateAssignmentModel model)
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == model.CourseId && 
            x.TeacherId == model.TeacherId, includeProperties: nameof(Course.CourseEnrollments));
            if (course == null)
            {
                throw new EntityNotFoundException($"Course with id {model.CourseId} and teacher id {model.TeacherId} was not found!");
            }

            var addedAssignment = await unitOfWork.AssignmentsRepository.AddAsync(new Assignment
            {
                CreatedAt = DateTime.UtcNow,
                CourseId = model.CourseId,
                Deadline = model.DueDate,
                Description = model.Description,
                Title = model.Title
            });
            
            var studentAssignments = course.CourseEnrollments.Select(x => new StudentAssignment
            {
                Assignment = addedAssignment,
                StudentId = x.StudentId
            }).ToList();
           
            await unitOfWork.StudentAssignmentsRepository.AddRangeAsync(studentAssignments);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid assignmentId)
        {
            await CheckIfAssignmentExistsAsync(assignmentId);

            await unitOfWork.AssignmentsRepository.RemoveAsync(assignmentId);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AssignmentDetailsModel>> GetAsync(Guid courseId, Guid studentId, AssignmentStatus status)
        {
            Expression<Func<StudentAssignment, bool>> filter = GetAssignmentsFilter(courseId, studentId, status);

            var assignments = (await unitOfWork.StudentAssignmentsRepository.GetAllAsync(filter, trackChanges: false))
            .Select(x => new AssignmentDetailsModel
            {
                StudentAssignmentId = x.Id,
                Deadline = x.Assignment.Deadline,
                Description = x.Assignment.Description,
                AssignmentId = x.Assignment.Id,
                Title = x.Assignment.Title,
                Points = x.Points
            })
            .ToList();

            return assignments;
        }

        private Expression<Func<StudentAssignment, bool>> GetAssignmentsFilter(Guid courseId, Guid studentId, AssignmentStatus status)
        {
            if (status == AssignmentStatus.Done)
            {
                return x => x.Assignment.CourseId == courseId && x.StudentId == studentId && x.CompletedAt != null;
            } 
            else if (status == AssignmentStatus.ToDo)
            {
                return x => x.Assignment.CourseId == courseId && x.StudentId == studentId && x.CompletedAt == null;
            }
            else
            {
                return x => x.Assignment.CourseId == courseId && x.StudentId == studentId;
            }
        }

        public async Task<List<ExtendedAssignmentDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            var assignments = (await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.CourseId == courseId,
                trackChanges: false))
                .Select(x => new ExtendedAssignmentDetailsModel
                {
                    Deadline = x.Deadline,
                    AssignmentId = x.Id,
                    Title = x.Title,
                    Students = x.StudentAssignments.Select(y => new GradeAssignmentModel
                    {
                        StudentId = y.StudentId,
                        StudentName = $"{y.Student.FirstName} {y.Student.LastName}",
                        Points = y.Points ?? 0
                    })
                    .ToList()
                })
                .ToList();

            return assignments;
        }

        public async Task GradeAssignmentAsync(SaveGradeModel model)
        {
            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x => x.AssignmentId ==
            model.AssignmentId && x.StudentId == model.StudentId);

            if (studentAssignment == null)
            {
                throw new EntityNotFoundException($"StudentAssignment entity with studentId {model.StudentId} and assignmentId {model.AssignmentId} " +
                    $"was not found!");
            }

            studentAssignment.Points = model.Points;
            await unitOfWork.StudentAssignmentsRepository.UpdateAsync(studentAssignment);
            await unitOfWork.SaveChangesAsync();
        }

        private async Task CheckIfAssignmentExistsAsync(Guid id)
        {
            bool assignmentExists = await unitOfWork.AssignmentsRepository.ExistsAsync(x => x.Id == id);

            if (!assignmentExists)
            {
                throw new EntityNotFoundException($"Assignment with id {id} was not found!");
            }
        }

        public async Task ResetSubmitDateAsync(Guid studentAssignmentId)
        {
            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x => x.Id == studentAssignmentId);
            if (studentAssignment == null)
            {
                throw new EntityNotFoundException($"StudentAssignment with id {studentAssignmentId} was not found!");
            }

            studentAssignment.CompletedAt = null;
            await unitOfWork.StudentAssignmentsRepository.UpdateAsync(studentAssignment);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
