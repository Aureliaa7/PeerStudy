using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.StudyGroupsAssignmentsComponent
{
    public partial class StudyGroupAssignments : PeerStudyComponentBase, IDisposable
    {
        [Parameter]
        public Guid StudyGroupId { get; set; }

        [Parameter]
        public string StudyGroupName { get; set;}


        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private List<FlatAssignmentModel> assignments = new List<FlatAssignmentModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                assignments = await AssignmentService.GetByStudyGroupAsync(StudyGroupId);
                await SetCurrentUserDataAsync();
                NavigationMenuService.AddStudyGroupNavigationMenuItems(StudyGroupId, StudyGroupName, currentUserRole.Value);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "The assignments could not be fetched...");
            }
        }

        private void RedirectToAssignmentDetailsPage(FlatAssignmentModel assignment)
        {
            NavigationManager.NavigateTo($"/{assignment.CourseTitle}/{assignment.CourseId}/{assignment.StudyGroupId}/{assignment.Id}/assignment-details");
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
