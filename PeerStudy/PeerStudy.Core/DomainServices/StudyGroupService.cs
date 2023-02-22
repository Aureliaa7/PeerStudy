using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
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

        public StudyGroupService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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

                groups.Add(studyGroup);
            }

            await unitOfWork.StudyGroupRepository.AddRangeAsync(groups);
            course.HasStudyGroups = true;
            await unitOfWork.CoursesRepository.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
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
                    Title = x.Name,
                    CourseTitle = course.Title,
                    Students = x.StudentStudyGroups.Select(x => new EnrolledStudentModel
                    {
                        FirstName = x.Student.FirstName,
                        LastName = x.Student.LastName
                    }).ToList()
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
                    CourseTitle = x.StudyGroup.Course.Title,
                    Students = x.StudyGroup.StudentStudyGroups.Select(x => new EnrolledStudentModel
                    {
                        FirstName = x.Student.FirstName,
                        LastName = x.Student.LastName
                    }).ToList(),
                    Title = x.StudyGroup.Name
                })
                .ToList();

            return studyGroups;
        }
    }
}
