using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.WorkItems;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.WorkItems.Components.WorkItemsListComponent
{
    public partial class WorkItemsList : PeerStudyComponentBase, IDisposable
    {
        [Inject]
        private IWorkItemService WorkItemService { get; set; }

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }


        [Parameter]
        public Guid StudyGroupId { get; set; }

        [Parameter]
        public string StudyGroupName { get; set; }


        private List<DropDownItem> studentDropDownItems = new List<DropDownItem>();
        private List<WorkItemStatus> workItemStatuses;
        private CreateUpdateWorkItemModel workItemModel = new CreateUpdateWorkItemModel();
        private List<WorkItemDetailsModel> workItems = new List<WorkItemDetailsModel>();

        // the delete button is visible if isReadOnly is false and the user has student role
        private WorkItemDetailsModel selectedRow;
        private bool showSelectedTaskDetails;
        private bool showAddWorkItemDialog;
        private bool showEditWorkItemDialog;
        private bool isReadOnly = false;

        private const string noWorkItemsMessage = "There are no work items yet..";

        protected override async Task OnInitializedAsync()
        {
           await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            await SetWorkItemDropItemsAsync();
            workItems = await WorkItemService.GetByStudyGroupAsync(StudyGroupId);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                NavigationMenuService.AddStudyGroupNavigationMenuItems(StudyGroupId, StudyGroupName);
            }
        }

        private async Task SetWorkItemDropItemsAsync()
        {
            workItemStatuses = Enum.GetValues(typeof(WorkItemStatus)).Cast<WorkItemStatus>().ToList();
            var students = await StudyGroupService.GetStudentsByGroupIdAsync(StudyGroupId);

            foreach (var student in students)
            {
                studentDropDownItems.Add(new DropDownItem
                {
                    Key = student.Id.ToString(),
                    Value = student.FullName
                });
            }
        }

        private bool AreButtonsDisabled()
        {
            return selectedRow == null;
        }

        private void ShowWorkItemDetails()
        {
            // display a popup with the details of the selected work item
            showSelectedTaskDetails = true;
        }

        // the btn should only be visible for students and editMode
        private async Task DeleteWorkItem()
        {
            try
            {
                var workItemToBeRemoved = workItems.First(x => x.Id == selectedRow.Id);
                await WorkItemService.DeleteAsync(selectedRow.Id);
                workItems = workItems.Except(new List<WorkItemDetailsModel> { workItemToBeRemoved }).ToList();
                selectedRow = null;
            }
            catch (Exception ex)
            {
                //TODO: show err message
            }
        }

        private async Task AddWorkItem()
        {
            showAddWorkItemDialog = false;
            workItemModel.StudyGroupId = StudyGroupId;
            try
            {
                var savedWorkItem = await WorkItemService.AddAsync(workItemModel);
                savedWorkItem.StudyGroupName = StudyGroupName;
                savedWorkItem.AssignedToFullName = studentDropDownItems.FirstOrDefault(x => x.Key == savedWorkItem.AssignedTo.ToString())?.Value;
                workItems.Add(savedWorkItem);
            }
            catch (Exception ex)
            {
                //TODO: show an err message

                // maybe add a template for peerbasecomponent
            }
            finally
            {
                workItemModel = new CreateUpdateWorkItemModel();
            }
        }

        private void CloseWorkItemDetailsPopup()
        {
            showSelectedTaskDetails = false;
            selectedRow = null;
        }

        private void CancelAddWorkItem()
        {
            showAddWorkItemDialog = false;
            workItemModel = new CreateUpdateWorkItemModel();
        }
        private void CancelEditWorkItem()
        {
            showEditWorkItemDialog = false;
            selectedRow = null;
            workItemModel = new CreateUpdateWorkItemModel();
        }

        private void EditWorkItem()
        {
            showEditWorkItemDialog = true;
            workItemModel = new CreateUpdateWorkItemModel
            {
                AssignedTo = selectedRow.AssignedTo,
                Description = selectedRow.Description,
                Status = selectedRow.Status,
                StudyGroupId = StudyGroupId,
                Title = selectedRow.Title
            };
        }

        private async Task SaveUpdatedWorkItem()
        {
            showEditWorkItemDialog = false;
            try
            {
                var workItemToBeUpdated = workItems.First(x => x.Id == selectedRow.Id);
                var updatedWorkItem = await WorkItemService.UpdateAsync(workItemModel, selectedRow.Id);

                workItemToBeUpdated.Title = workItemModel.Title;
                workItemToBeUpdated.Description = workItemModel.Description;
                workItemToBeUpdated.Status = workItemModel.Status;
                workItemToBeUpdated.AssignedTo = workItemModel.AssignedTo;
                workItemToBeUpdated.AssignedToFullName = (studentDropDownItems.First(x => x.Key == workItemModel.AssignedTo.ToString()))?.Value;
                selectedRow = null;
                StateHasChanged();
            }
            catch (Exception e)
            {
                //TODO: show an err message
            }
            finally
            {
                workItemModel = new CreateUpdateWorkItemModel();
            }
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
