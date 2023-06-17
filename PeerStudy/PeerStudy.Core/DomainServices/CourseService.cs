using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGoogleDriveFileService fileService;
        private readonly IGoogleDrivePermissionService permissionService;
        private readonly IConfigurationService configurationService;

        private const string assignmentsFolderName = "Assignments";
        private const string resourcesFolderName = "Resources";
        private const string studyGroupsFolderName = "StudyGroups";

        public CourseService(IUnitOfWork unitOfWork,
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService permissionService,
            IConfigurationService configurationService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
            this.permissionService = permissionService;
            this.configurationService = configurationService;
        }

        public async Task<CourseDetailsModel> AddAsync(CourseModel courseModel)
        {
            var teacher = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == courseModel.TeacherId && 
            x.Role == Role.Teacher);

            if (teacher == null)
            {
                throw new EntityNotFoundException($"Teacher with id {courseModel.TeacherId} was not found!");
            }

            var course = new Course
            {
                Title = courseModel.Title,
                StartDate = courseModel.StartDate,
                EndDate = courseModel.EndDate,
                NoStudents = courseModel.NumberOfStudents,
                TeacherId = teacher.Id,
                Status = CourseStatus.Active
            };

            course.DriveRootFolderId = await fileService.CreateFolderAsync(courseModel.Title);
            course.AssignmentsDriveFolderId = await fileService.CreateFolderAsync(assignmentsFolderName, course.DriveRootFolderId);
            course.ResourcesDriveFolderId = await fileService.CreateFolderAsync(resourcesFolderName, course.DriveRootFolderId);
            course.StudyGroupsDriveFolderId = await fileService.CreateFolderAsync(studyGroupsFolderName, course.DriveRootFolderId);

            await permissionService.SetPermissionsAsync(new List<string> { course.ResourcesDriveFolderId }, 
                new List<string> { teacher.Email }, "writer");
            await permissionService.SetPermissionsAsync(new List<string> { course.DriveRootFolderId }, 
                new List<string> { configurationService.AppEmail }, "writer");

            var insertedCourse = await unitOfWork.CoursesRepository.AddAsync(course);
            await unitOfWork.SaveChangesAsync();

            return new CourseDetailsModel
            {
                Id = insertedCourse.Id,
                NoMaxStudents = insertedCourse.NoStudents,
                StartDate = insertedCourse.StartDate,
                EndDate= insertedCourse.EndDate,
                Title = insertedCourse.Title,
                TeacherName = $"{teacher.FirstName} {teacher.LastName}",
                Status = insertedCourse.Status,
                TeacherId = insertedCourse.TeacherId
            };
        }

        public async Task<bool> ArchiveCourseAsync(Guid teacherId, Guid courseId)
        {
            await CheckIfTeacherExistsAsync(teacherId);
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(
                x => x.TeacherId == teacherId && x.Id == courseId);
            if (course == null)
            {
                throw new EntityNotFoundException($"The course with id: {courseId} was not found!");
            }

            course.Status = CourseStatus.Archived;
            await unitOfWork.CoursesRepository.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<CourseDetailsModel>> GetAsync(Guid teacherId, CourseStatus status)
        {
            await CheckIfTeacherExistsAsync(teacherId);

            var courses = (await unitOfWork.CoursesRepository.GetAllAsync(x => x.TeacherId == teacherId &&
            x.Status == status, includeProperties: nameof(Teacher))).Select(
                x => new CourseDetailsModel
                {
                    Id = x.Id,
                    NoMaxStudents = x.NoStudents,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Title = x.Title,
                    TeacherName = $"{x.Teacher.FirstName} {x.Teacher.LastName}",
                    Status = x.Status,
                    TeacherId = x.TeacherId,
                    HasStudyGroups = x.HasStudyGroups,
                    NoEnrolledStudents = x.CourseEnrollments.Count,
                    NoCourseUnits = x.CourseUnits.Count
                })
            .ToList();

            return courses;
        }

        public async Task<CourseDetailsModel> UpdateAsync(UpdateCourseModel courseModel)
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseModel.Id &&
            x.TeacherId == courseModel.TeacherId);

            if (course == null)
            {
                throw new EntityNotFoundException();
            }

            course.StartDate = courseModel.StartDate;
            course.EndDate = courseModel.EndDate;
            course.Title = courseModel.Title;
            course.NoStudents = courseModel.NumberOfStudents;

            await unitOfWork.CoursesRepository.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();

            return MapToCourseDetails(course);
        }

        private async Task CheckIfTeacherExistsAsync(Guid teacherId)
        {
            bool teacherExists = await unitOfWork.UsersRepository.ExistsAsync(x => x.Id == teacherId && x.Role == Role.Teacher);

            if (!teacherExists)
            {
                throw new EntityNotFoundException($"Teacher with id {teacherId} was not found!");
            }
        }

        private CourseDetailsModel MapToCourseDetails(Course course)
        {
            return new CourseDetailsModel
            {
                Id = course.Id,
                NoMaxStudents = course.NoStudents,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Title = course.Title,
                TeacherName = $"{course.Teacher?.FirstName} {course.Teacher?.LastName}",
                Status = course.Status,
                HasStudyGroups = course.HasStudyGroups,
                NoEnrolledStudents = course.CourseEnrollments.Count
            };
        }

        public async Task<List<CourseDetailsModel>> GetCoursesToEnroll(Guid studentId)
        {
            var excludedCoursesIds = (await unitOfWork.CourseEnrollmentRequestsRepository.GetAllAsync(
                x => x.StudentId == studentId))
                .Select(x => x.CourseId)
                .ToList();

            var courses = (await unitOfWork.CoursesRepository.GetAllAsync(x => x.StartDate.Date > DateTime.Today.Date &&
                    x.Status == CourseStatus.Active && 
                    !excludedCoursesIds.Contains(x.Id) && 
                    x.CourseEnrollments.Count < x.NoStudents, includeProperties: nameof(Teacher)))
                     .Select(
                x => new CourseDetailsModel
                {
                    Id = x.Id,
                    NoMaxStudents = x.NoStudents,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Title = x.Title,
                    TeacherName = $"{x.Teacher.FirstName} {x.Teacher.LastName}",
                    Status = x.Status,
                    TeacherId = x.TeacherId,
                    HasStudyGroups = x.HasStudyGroups,
                    NoEnrolledStudents = x.CourseEnrollments.Count
                })
                .ToList();

            return courses;
        }

        public async Task<List<CourseDetailsModel>> GetCoursesForStudentAsync(Guid studentId, CourseStatus status)
        {
            var courses = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.StudentId == studentId
                    && x.Course.Status == status, trackChanges: false))
                .Select(
                x => new CourseDetailsModel
                    {
                        Id = x.CourseId,
                        NoMaxStudents = x.Course.NoStudents,
                        StartDate = x.Course.StartDate,
                        EndDate = x.Course.EndDate,
                        Title = x.Course.Title,
                        TeacherName = $"{x.Course.Teacher.FirstName} {x.Course.Teacher.LastName}",
                        Status = x.Course.Status,
                        TeacherId = x.Course.TeacherId,
                        HasStudyGroups = x.Course.HasStudyGroups,
                        NoEnrolledStudents = x.Course.CourseEnrollments.Count,
                        NoCourseUnits = x.Course.CourseUnits.Count
                    })
                .ToList();

            return courses;
        }

        public async Task<CourseDetailsModel> GetDetailsAsync(Guid courseId)
        {
            var course = await GetByIdAsync(courseId, nameof(Teacher));

            return MapToCourseDetails(course);
        }

        public async Task<List<EnrolledStudentModel>> GetStudentsAsync(Guid courseId)
        {
            var course = await GetByIdAsync(courseId, $"{nameof(Course.CourseEnrollments)}.{nameof(StudentCourse.Student)}");

            var students = course.CourseEnrollments.Select(x => new EnrolledStudentModel
            {
                FirstName = x.Student.FirstName,
                LastName = x.Student.LastName
            })
            .ToList();

            return students;
        }

        public async Task<CourseStatus> GetCourseStatusAsync(Guid courseId)
        {
            var course = await GetByIdAsync(courseId);

            return course.Status;
        }

        private async Task<Course> GetByIdAsync(Guid id, string propertiesToBeIncluded = "")
        {
            var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: propertiesToBeIncluded);
            if (course == null)
            {
                throw new EntityNotFoundException($"Course with id: {id} was not found!");
            }

            return course;
        }
    }
}
 