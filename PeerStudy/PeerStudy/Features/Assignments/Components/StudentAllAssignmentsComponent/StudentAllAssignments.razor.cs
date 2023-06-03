using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.StudentAllAssignmentsComponent
{
    public partial class StudentAllAssignments : PeerStudyComponentBase
    {
        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private List<FlatAssignmentModel> assignments = new List<FlatAssignmentModel>();
        private List<AssignmentStatus?> assignmentStatuses = new List<AssignmentStatus?>();
        private AssignmentStatus? assignmentStatus = AssignmentStatus.Upcoming;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            assignments = await AssignmentService.GetByStudentAsync(currentUserId, AssignmentStatus.Upcoming);
            assignmentStatuses = Enum.GetValues(typeof(AssignmentStatus)).Cast<AssignmentStatus?>().ToList();
        }

        private async Task HandleSelectedStatusChanged(AssignmentStatus? newStatus)
        {
            isLoading = true;
            assignmentStatus = newStatus;
            assignments.Clear();
            StateHasChanged();
            try
            {
                assignments = await AssignmentService.GetByStudentAsync(currentUserId, newStatus.Value);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while fetching the assignments...");
            }
            isLoading = false;
        }

        private void HandleClickedAssignment(FlatAssignmentModel assignment)
        {
            NavigationManager.NavigateTo($"/{assignment.CourseTitle}/{assignment.CourseId}/{assignment.StudyGroupId}/{assignment.Id}/assignment-details");
        }
    }
}
