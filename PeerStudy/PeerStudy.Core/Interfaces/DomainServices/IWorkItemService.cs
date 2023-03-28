using PeerStudy.Core.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IWorkItemService
    {
        Task<List<WorkItemDetailsModel>> GetByStudyGroupAsync(Guid studyGroupId);

        Task<WorkItemDetailsModel> AddAsync(CreateUpdateWorkItemModel createWorkItemModel);

        Task<WorkItemDetailsModel> UpdateAsync(CreateUpdateWorkItemModel updateWorkItemModel, Guid id);

        Task DeleteAsync(Guid id);
    }
}
