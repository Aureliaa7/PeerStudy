using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Emails;
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
        private readonly IEmailService emailService;

        public WorkItemService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
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
                AssignedTo = student,
                ChangedById = createWorkItemModel.ChangedBy
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
            var workItem = await unitOfWork.WorkItemsRepository.GetFirstOrDefaultAsync(x => x.Id == id)
                ?? throw new EntityNotFoundException($"Work item with id {id} was not found!");
            Guid? oldWorkItemResponsibleId = workItem.AssignedToId;

            workItem.Description = updateWorkItemModel.Description;
            workItem.AssignedToId = updateWorkItemModel.AssignedTo;
            workItem.Title = updateWorkItemModel.Title;
            workItem.Status = updateWorkItemModel.Status;
            workItem.ChangedById = updateWorkItemModel.ChangedBy;

            await unitOfWork.WorkItemsRepository.UpdateAsync(workItem);
            await unitOfWork.SaveChangesAsync();

            await NotifyWorkItemResponsibleAsync(updateWorkItemModel.StudyGroupId, updateWorkItemModel.ChangedBy,
                updateWorkItemModel.Title, oldWorkItemResponsibleId, updateWorkItemModel.AssignedTo);

            return new WorkItemDetailsModel
            {
                Id = workItem.Id,
                AssignedTo = workItem.AssignedToId,
                Description = workItem.Description,
                Status = workItem.Status,
                StudyGroupId = workItem.StudyGroupId,
                Title = workItem.Title,
                ChangedBy = workItem.ChangedById
            };
        }

        private async Task NotifyWorkItemResponsibleAsync(Guid studyGroupId, 
            Guid changedById,
            string workItemTitle,
            Guid? assignedToId, 
            Guid? newAssignedToId)
        {
            if (assignedToId == null && newAssignedToId == null ||
                assignedToId == null && newAssignedToId != null && changedById == newAssignedToId ||
                assignedToId != null && newAssignedToId == null && changedById == assignedToId.Value)
            {
                return;
            }

            try
            {
                var changedBy = await GetUserByIdAsync(changedById);
                var studyGroup = await unitOfWork.StudyGroupRepository.GetFirstOrDefaultAsync(x => x.Id == studyGroupId,
                    includeProperties: nameof(Course)) ?? throw new EntityNotFoundException($"Study group witn id {studyGroupId} was not found!");

                if (assignedToId == null && newAssignedToId != null)
                {
                    var assignedTo = await GetUserByIdAsync(newAssignedToId.Value);
                    
                    var emailModel = new AssignWorkItemEmailModel
                    {
                        ChangedBy = $"{changedBy.FirstName} {changedBy.LastName}",
                        CourseTitle = studyGroup.Course.Title,
                        EmailType = EmailType.AssignWorkItem,
                        RecipientName = $"{assignedTo.FirstName} {assignedTo.LastName}",
                        StudyGroupTitle = studyGroup.Name,
                        WorkItemTitle = workItemTitle,
                        To = new List<string> { assignedTo.Email }
                    };

                    await emailService.SendAsync(emailModel);
                }

                else if (assignedToId != null && newAssignedToId == null)
                {
                    var assignedTo = await GetUserByIdAsync(assignedToId.Value);
                
                    var emailModel = new AssignWorkItemEmailModel
                    {
                        ChangedBy = $"{changedBy.FirstName} {changedBy.LastName}",
                        CourseTitle = studyGroup.Course.Title,
                        EmailType = EmailType.UnassignWorkItem,
                        RecipientName = $"{assignedTo.FirstName} {assignedTo.LastName}",
                        StudyGroupTitle = studyGroup.Name,
                        WorkItemTitle = workItemTitle,
                        To = new List<string> { assignedTo.Email }
                    };

                    await emailService.SendAsync(emailModel);
                }
                else if (assignedToId != null && newAssignedToId != null && assignedToId != newAssignedToId)
                {
                    var unassignedFrom = await GetUserByIdAsync(assignedToId.Value);
                    var assignedTo = await GetUserByIdAsync(newAssignedToId.Value);
            
                    var assignWorItemEmailModel = new AssignWorkItemEmailModel
                    {
                        ChangedBy = $"{changedBy.FirstName} {changedBy.LastName}",
                        CourseTitle = studyGroup.Course.Title,
                        EmailType = EmailType.AssignWorkItem,
                        RecipientName = $"{assignedTo.FirstName} {assignedTo.LastName}",
                        StudyGroupTitle = studyGroup.Name,
                        WorkItemTitle = workItemTitle,
                        To = new List<string> { assignedTo.Email }
                    };
                    await emailService.SendAsync(assignWorItemEmailModel);

                    var unassignWorkItemEmailModel = new UnassignWorkItemEmailModel
                    {
                        ChangedBy = $"{changedBy.FirstName} {changedBy.LastName}",
                        CourseTitle = studyGroup.Course.Title,
                        EmailType = EmailType.UnassignWorkItem,
                        RecipientName = $"{unassignedFrom.FirstName} {unassignedFrom.LastName}",
                        StudyGroupTitle = studyGroup.Name,
                        WorkItemTitle = workItemTitle,
                        To = new List<string> { unassignedFrom.Email }
                    };
                    await emailService.SendAsync(unassignWorkItemEmailModel);
                }
            }
            catch { }
        }

        private async Task<User> GetUserByIdAsync(Guid id)
        {
            return await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == id)
                ?? throw new EntityNotFoundException($"User with id {id} was not found!");
        }
    }
}
