using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseService
    {
        Task<CourseDetailsModel> AddAsync(CreateCourseModel courseModel);

        Task<List<CourseDetailsModel>> GetAsync(Guid teacherId, CourseStatus status);

        Task<bool> ArchiveCourseAsync(Guid teacherId, Guid courseId);
    }
}
