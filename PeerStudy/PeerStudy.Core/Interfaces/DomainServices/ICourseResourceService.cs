using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseResourceService
    {
        Task<List<ResourceDetailsModel>> UploadResourcesAsync(UploadCourseResourcesModel resources);

        Task<List<ResourceDetailsModel>> GetByCourseIdAsync(Guid courseId);

        Task DeleteAsync(Guid resourceId);
    }
}
