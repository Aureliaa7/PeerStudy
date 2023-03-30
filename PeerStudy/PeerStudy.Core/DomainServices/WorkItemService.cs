using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IUnitOfWork unitOfWork;

        public WorkItemService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<WorkItemDetailsModel> AddAsync(CreateUpdateWorkItemModel createWorkItemModel)
        {
            Student student = null;
            if (createWorkItemModel.AssignedTo != null)
            {
                student = (Student)await unitOfWork.UsersRepository.GetByIdAsync(createWorkItemModel.AssignedTo.Value);
            }

            var workItem = new WorkItem
            {
                AssignedToId = createWorkItemModel.AssignedTo,
                CreatedAt = DateTime.UtcNow,
                Description = createWorkItemModel.Description,
                Status = createWorkItemModel.Status,
                Title = createWorkItemModel.Title,
                StudyGroupId = createWorkItemModel.StudyGroupId,
                AssignedTo = student
            };

            var savedWorkItem = await unitOfWork.WorkItemsRepository.AddAsync(workItem);
            await unitOfWork.SaveChangesAsync();

            return new WorkItemDetailsModel
            {
                Id = savedWorkItem.Id,
                AssignedTo = savedWorkItem.AssignedToId,
                Description = savedWorkItem.Description,
                Status = savedWorkItem.Status,
                StudyGroupId = savedWorkItem.StudyGroupId,
                Title = savedWorkItem.Title
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var workItemExists = await unitOfWork.WorkItemsRepository.ExistsAsync(x => x.Id == id);
            if (!workItemExists)
            {
                throw new EntityNotFoundException($"Work item with id {id} was not found!");
            }

            await unitOfWork.WorkItemsRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<WorkItemDetailsModel>> GetByStudyGroupAsync(Guid studyGroupId) 
        {
            var workItems = (await unitOfWork.WorkItemsRepository.GetAllAsync(x => x.StudyGroupId == studyGroupId))
                .Select(x => new WorkItemDetailsModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    AssignedToFullName = $"{x.AssignedTo.FirstName} {x.AssignedTo.LastName}",
                    Status = x.Status,
                    AssignedTo = x.AssignedTo.Id,
                    StudyGroupId = studyGroupId,
                    StudyGroupName = x.StudyGroup.Name,
                    Title = x.Title
                })
                .ToList();

            return workItems;
        }

        public async Task<WorkItemDetailsModel> UpdateAsync(CreateUpdateWorkItemModel updateWorkItemModel, Guid id)
        {
            var workItem = await unitOfWork.WorkItemsRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (workItem == null)
            {
                throw new EntityNotFoundException($"Work item with id {id} was not found!");
            }

            workItem.Description = updateWorkItemModel.Description;
            workItem.AssignedToId = updateWorkItemModel.AssignedTo;
            workItem.Title = updateWorkItemModel.Title;
            workItem.Status = updateWorkItemModel.Status;

            await unitOfWork.WorkItemsRepository.UpdateAsync(workItem);
            await unitOfWork.SaveChangesAsync();

            return new WorkItemDetailsModel
            {
                Id = workItem.Id,
                AssignedTo = workItem.AssignedToId,
                Description = workItem.Description,
                Status = workItem.Status,
                StudyGroupId = workItem.StudyGroupId,
                Title = workItem.Title
            };
        }
    }
}
