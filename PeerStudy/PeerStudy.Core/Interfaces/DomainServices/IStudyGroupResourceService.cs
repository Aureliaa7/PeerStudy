using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudyGroupResourceService
    {
        Task<List<ResourceDetailsModel>> GetByStudyGroupIdAsync(Guid id);

        Task<List<ResourceDetailsModel>> UploadAsync(UploadStudyGroupResourceModel uploadResourceModel);

        Task DeleteAsync(Guid id);
    }
}
