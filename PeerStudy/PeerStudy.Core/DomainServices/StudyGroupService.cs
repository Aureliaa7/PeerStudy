using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.StudyGroups;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class StudyGroupService : IStudyGroupService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGoogleDriveFileService driveFileService;

        public StudyGroupService(IUnitOfWork unitOfWork, IGoogleDriveFileService driveFileService)
        {
            this.unitOfWork = unitOfWork;
            this.driveFileService = driveFileService;
        }

        public async Task CreateStudyGroupsAsync(Guid teacherId, Guid courseId, int noStudentsPerGroup)
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseId && x.TeacherId == teacherId,
                includeProperties: $"{nameof(Course.CourseEnrollments)}.{nameof(StudentCourse.Student)}");
            if (course == null)
            {
                throw new EntityNotFoundException($"Course with id {courseId} and teacher id {teacherId} was not found!");
            }

            if (course.HasStudyGroups)
            {
                throw new PreconditionFailedException($"Study groups have already been created for course with id {courseId}");
            }

            if (course.StartDate > DateTime.UtcNow)
            {
                throw new PreconditionFailedException("Cannot create study groups before the start of the course!");
            }

            var enrolledStudents = course.CourseEnrollments
                .Select(x => x.Student)
                .OrderBy(x => Guid.NewGuid())
                .ToList();

            var groupsList = enrolledStudents
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / noStudentsPerGroup)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();

            var groups = new List<StudyGroup>();
            int i = 1;
            foreach (var group in groupsList)
            {
                var studyGroup = new StudyGroup
                {
                    CourseId = courseId,
                    Name = $"Group {i++}",
                    StudentStudyGroups = new List<StudentStudyGroup>()
                };

                foreach (var student in group)
                {
                    studyGroup.StudentStudyGroups.Add(new StudentStudyGroup
                    {
                        StudentId = student.Id,
                        StudyGroup = studyGroup
                    });
                }

                studyGroup.DriveFolderId = await driveFileService.CreateFolderAsync(studyGroup.Name, course.StudyGroupsDriveFolderId);
                groups.Add(studyGroup);
            }

            await unitOfWork.StudyGroupRepository.AddRangeAsync(groups);
            course.HasStudyGroups = true;
            await unitOfWork.CoursesRepository.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<StudyGroupDetailsModel> GetAsync(Guid id)
        {
            var studyGroup = await GetStudyGroupAsync(id, 
                $"{nameof(StudyGroup.StudentStudyGroups)}.{nameof(StudentStudyGroup.Student)},{nameof(StudyGroup.Course)}");

            return new StudyGroupDetailsModel
            {
                Id = studyGroup.Id,
                Title = studyGroup.Name,
                CourseTitle = studyGroup.Course.Title,
                Students = studyGroup.StudentStudyGroups.Select(x => new EnrolledStudentModel
                {
                    FirstName = x.Student.FirstName,
                    LastName = x.Student.LastName
                }).ToList(),
                IsActive = studyGroup.Course.Status == CourseStatus.Active
            };
        }

        public async Task<List<StudyGroupDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseId);
            if (course == null)
            {
                throw new EntityNotFoundException($"Course with id {courseId} was not found!");
            }

            var studyGroups = (await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.CourseId == courseId,
                includeProperties: $"{nameof(StudyGroup.StudentStudyGroups)}.{nameof(StudentStudyGroup.Student)}", trackChanges: false))
                .Select(x => new StudyGroupDetailsModel
                {
                    Id = x.Id,
                    Title = x.Name,
                    CourseTitle = course.Title,
                    Students = x.StudentStudyGroups.Select(x => new EnrolledStudentModel
                    {
                        FirstName = x.Student.FirstName,
                        LastName = x.Student.LastName
                    }).ToList(),
                    IsActive = course.Status == CourseStatus.Active
                })
                .ToList();

            return studyGroups;
        }

        public async Task<List<StudyGroupDetailsModel>> GetByStudentIdAsync(Guid studentId)
        {
            var studyGroups = (await unitOfWork.StudentStudyGroupRepository.GetAllAsync(
                x => x.StudentId == studentId, includeProperties: $"{nameof(StudentStudyGroup.Student)}," +
                $"{nameof(StudentStudyGroup.StudyGroup)}.{nameof(StudentStudyGroup.StudyGroup.StudentStudyGroups)}"))
                .Select(x => new StudyGroupDetailsModel
                {
                    Id = x.StudyGroupId,
                    CourseTitle = x.StudyGroup.Course.Title,
                    Students = x.StudyGroup.StudentStudyGroups.Select(x => new EnrolledStudentModel
                    {
                        FirstName = x.Student.FirstName,
                        LastName = x.Student.LastName
                    }).ToList(),
                    Title = x.StudyGroup.Name,
                    IsActive = x.StudyGroup.Course.Status == CourseStatus.Active
                })
                .ToList();

            return studyGroups;
        }

        public async Task<List<UserModel>> GetStudentsByGroupIdAsync(Guid id)
        {
            var studyGroup = await GetStudyGroupAsync(id,
               $"{nameof(StudyGroup.StudentStudyGroups)}.{nameof(StudentStudyGroup.Student)}");

            return studyGroup
                .StudentStudyGroups
                .Select(x => new UserModel
                    {
                        FullName = $"{x.Student.FirstName} {x.Student.LastName}",
                        Id = x.Student.Id
                    })
                .ToList();
        }

        public async Task<bool> IsActiveAsync(Guid id)
        {
            var studyGroup = await unitOfWork.StudyGroupRepository.GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: nameof(Course));

            return studyGroup?.Course?.Status == CourseStatus.Active;
        }

        private async Task<StudyGroup> GetStudyGroupAsync(Guid id, string propertiesToBeIncluded)
        {
            var studyGroup = await unitOfWork.StudyGroupRepository.GetFirstOrDefaultAsync(x => x.Id == id,
              includeProperties: propertiesToBeIncluded);

            if (studyGroup == null)
            {
                throw new EntityNotFoundException($"Study group with id {id} was not found!");
            }
           
            return studyGroup;
        }
    }
}
