using Blazored.Toast.Services;
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
    public partial class WorkItemsList : PeerStudyComponentBase
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
        private List<WorkItemStatus?> workItemStatusesForFiltering;
        private List<WorkItemStatus> workItemStatuses;
        private CreateUpdateWorkItemModel workItemModel = new CreateUpdateWorkItemModel();
        private List<WorkItemDetailsModel> allWorkItems = new List<WorkItemDetailsModel>();
        private List<WorkItemDetailsModel> filteredWorkItems = new List<WorkItemDetailsModel>();

        private WorkItemDetailsModel selectedRow;
        private bool showSelectedTaskDetails;
        private bool showAddWorkItemDialog;
        private bool showEditWorkItemDialog;
        private bool isReadOnly;

        private string? selectedStudentIdFilter;
        private WorkItemStatus? selectedWorkItemStatusFilter;
        private DropDownItem? selectedStudent; // used for edit 

        private const string noWorkItemsMessage = "There are no work items yet..";
        private const string dropdownStyles = "width: 90%;";

        protected override async Task OnInitializedAsync()
        {
           await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await SetCurrentUserDataAsync();
                await SetWorkItemDropItemsAsync();
                allWorkItems = await WorkItemService.GetByStudyGroupAsync(StudyGroupId);
                filteredWorkItems = allWorkItems;
                isReadOnly = !await StudyGroupService.IsActiveAsync(StudyGroupId);
            } 
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred...");
            }
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
            workItemStatusesForFiltering = workItemStatuses.Select(x => (WorkItemStatus?)x).ToList();
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
            showSelectedTaskDetails = true;
        }

        private async Task DeleteWorkItem()
        {
            try
            {
                var workItemToBeRemoved = allWorkItems.First(x => x.Id == selectedRow.Id);
                await WorkItemService.DeleteAsync(selectedRow.Id);
                allWorkItems = allWorkItems.Except(new List<WorkItemDetailsModel> { workItemToBeRemoved }).ToList();
                selectedRow = null;
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "The work item could not be deleted...");
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
                allWorkItems.Add(savedWorkItem);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while creating the work item...");
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
            selectedStudent = studentDropDownItems.FirstOrDefault(x => new Guid(x.Key) == selectedRow.AssignedTo);
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
                var workItemToBeUpdated = allWorkItems.First(x => x.Id == selectedRow.Id);
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while updating the work item..."); 
            }
            finally
            {
                workItemModel = new CreateUpdateWorkItemModel();
            }
        }

        private Task HandleSelectedWorkItemStatusChanged(WorkItemStatus? status)
        {
            selectedWorkItemStatusFilter = status;
            FilterWorkItems();

            return Task.CompletedTask;
        }

        private Task HandleSelectedStudentChanged(string studentId)
        {
            selectedStudentIdFilter = studentId;
            FilterWorkItems();

            return Task.CompletedTask;
        }

        private void FilterWorkItems()
        {
            var filter = GetFilter();
            if (filter != null)
            {
                filteredWorkItems = allWorkItems.Where(filter)
                    .ToList();
            }
            else
            {
                filteredWorkItems = allWorkItems;
            }
        }

        private Func<WorkItemDetailsModel, bool> GetFilter()
        {
            Func<WorkItemDetailsModel, bool> filter = null;

            if (!string.IsNullOrWhiteSpace(selectedStudentIdFilter) && selectedWorkItemStatusFilter != null)
            {
                filter = (x) => x.AssignedTo == new Guid(selectedStudentIdFilter) && x.Status == selectedWorkItemStatusFilter;
            }
            else if (!string.IsNullOrWhiteSpace(selectedStudentIdFilter) && selectedWorkItemStatusFilter == null)
            {
                filter = (x) => x.AssignedTo == new Guid(selectedStudentIdFilter);
            }
            else if (string.IsNullOrWhiteSpace(selectedStudentIdFilter) && selectedWorkItemStatusFilter != null)
            {
                filter = (x) => x.Status == selectedWorkItemStatusFilter;
            }

            return filter;
        }

        private void ResetFilters()
        {
            selectedWorkItemStatusFilter = null;
            selectedStudentIdFilter = null;
            filteredWorkItems = allWorkItems;
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);
            NavigationMenuService.Reset();
        }
    }
}
