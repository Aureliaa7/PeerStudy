﻿using PeerStudy.Core.DomainEntities;
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
                StudentGroupId = x.Assignment.StudyGroupId,
                Deadline = x.Assignment.Deadline,
                Description = x.Assignment.Description,
                Id = x.Assignment.Id,
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
                return x => x.Assignment.CourseUnit.CourseId == courseId && x.StudentId == studentId && x.Assignment.CompletedAt != null;
            }
            else if (status == AssignmentStatus.ToDo)
            {
                return x => x.Assignment.CourseUnit.CourseId == courseId && x.StudentId == studentId && x.Assignment.CompletedAt == null;
            }
            else
            {
                return x => x.Assignment.CourseUnit.CourseId == courseId && x.StudentId == studentId;
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

        public async Task ResetSubmitDateAsync(Guid assignmentId)
        {
            var assignment = await unitOfWork.AssignmentsRepository.GetFirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assignment == null)
            {
                throw new EntityNotFoundException($"Assignment with id {assignmentId} was not found!");
            }

            assignment.CompletedAt = null;
            await unitOfWork.AssignmentsRepository.UpdateAsync(assignment);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
