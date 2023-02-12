using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseService
    {
        Task<CourseDetailsModel> AddAsync(CourseModel courseModel);

        Task<List<CourseDetailsModel>> GetAsync(Guid teacherId, CourseStatus status);

        Task<bool> ArchiveCourseAsync(Guid teacherId, Guid courseId);

        Task<CourseDetailsModel> UpdateAsync(UpdateCourseModel courseModel);

        /// <summary>
        /// Returns a list of courses a student can enroll in
        /// </summary>
        /// <param name="studentId">The student id</param>
        /// <returns></returns>
        Task<List<CourseDetailsModel>> GetCoursesToEnroll(Guid studentId);

        Task<List<CourseDetailsModel>> GetCoursesForStudentAsync(Guid studentId, CourseStatus status);

        Task<CourseDetailsModel> GetDetailsAsync(Guid courseId);
    }
}
