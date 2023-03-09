using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Assignments
{
    public partial class StudentCourseAssignments
    {
        [Inject]
        public IAssignmentService AssignmentService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private List<AssignmentDetailsModel> doneAssignments = new List<AssignmentDetailsModel>();
        private List<AssignmentDetailsModel> toDoAssignments = new List<AssignmentDetailsModel>();
        private bool isLoading;
        private const string noAssignmentsMessage = "There are no assignments yet...";
        private const string doneAssignmentsTitle = "Done";
        private const string toDoAssignmentsTitle = "To do";

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            doneAssignments = await AssignmentService.GetAsync(CourseId, StudentId, AssignmentStatus.Done);
            toDoAssignments = await AssignmentService.GetAsync(CourseId, StudentId, AssignmentStatus.ToDo);
            isLoading = false;
        }

        private void HandleClickedAssignment(Guid assignmentId)
        {
            NavigationManager.NavigateTo($"/{CourseTitle}/{CourseId}/{StudentId}/{assignmentId}/assignment-details");
        }
    }
}
