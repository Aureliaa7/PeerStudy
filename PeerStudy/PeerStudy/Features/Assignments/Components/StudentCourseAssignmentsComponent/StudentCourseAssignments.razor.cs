﻿using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.StudentCourseAssignmentsComponent
{
    public partial class StudentCourseAssignments : PeerStudyComponentBase, IDisposable
    {
        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private List<AssignmentDetailsModel> doneAssignments = new List<AssignmentDetailsModel>();
        private List<AssignmentDetailsModel> toDoAssignments = new List<AssignmentDetailsModel>();
        private const string doneAssignmentsTitle = "Done";
        private const string toDoAssignmentsTitle = "To do";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                doneAssignments = await AssignmentService.GetByCourseAndStudentAsync(CourseId, StudentId, AssignmentStatus.Done);
                toDoAssignments = await AssignmentService.GetByCourseAndStudentAsync(CourseId, StudentId, AssignmentStatus.Upcoming);

                await SetCurrentUserDataAsync();
                NavigationMenuService.AddCourseNavigationMenuItems(
                currentUserId,
                CourseId,
                CourseTitle,
                currentUserRole);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "The assignments could not be fetched...");
            }
        }

        private void HandleClickedAssignment(AssignmentDetailsModel assignment)
        {
            NavigationManager.NavigateTo($"/{CourseTitle}/{CourseId}/{assignment.StudentGroupId}/{assignment.Id}/assignment-details");
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
