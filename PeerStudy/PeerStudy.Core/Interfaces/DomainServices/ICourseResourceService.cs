using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseResourceService
    {
        Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(UploadCourseResourcesModel resources);

        Task<List<CourseResourceDetailsModel>> GetByCourseIdAsync(Guid courseId);

        Task DeleteAsync(Guid resourceId);

        Task DeleteRangeAsync(List<Guid> resourcesIds);
    }
}
