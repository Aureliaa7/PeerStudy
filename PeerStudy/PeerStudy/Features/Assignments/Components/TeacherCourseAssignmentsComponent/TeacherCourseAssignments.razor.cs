using Blazored.Toast.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Features.Courses.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.TeacherCourseAssignmentsComponent
{
    public partial class TeacherCourseAssignments: PeerStudyComponentBase
    {
        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject] 
        private IStateSelection<CoursesState, CourseDetailsModel> SelectedCourse { get; set; }

        [Inject]
        private IState<CoursesState> CoursesState { get; set; }


        [Parameter]
        public Guid CourseUnitId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        private List<ExtendedAssignmentDetailsModel> assignments = new List<ExtendedAssignmentDetailsModel>();
        private const string noAssignmentsMessage = "There are no assignments yet...";
        private bool isReadOnly;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                CheckIfIsReadOnlyMode();

                await SetCurrentUserDataAsync();
                assignments = await AssignmentService.GetByCourseUnitIdAsync(CourseUnitId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred...");
            }
        }

        private void CheckIfIsReadOnlyMode()
        {
            var course = CoursesState.Value.ActiveCourses.FirstOrDefault(x => x.Id == CourseId) ??
                   CoursesState.Value.ArchivedCourses.FirstOrDefault(x => x.Id == CourseId);

            isReadOnly = course.Status == CourseStatus.Archived;

            SelectedCourse.Select(x => x.ActiveCourses.FirstOrDefault(y => y.Id == CourseId) ??
            x.ArchivedCourses.FirstOrDefault(y => y.Id == CourseId));

            SelectedCourse.SelectedValueChanged += (object? sender, CourseDetailsModel course) =>
            {
                isReadOnly = course.Status == CourseStatus.Archived;
            };
        }

        private async Task SaveGradeHandler(SaveGradeModel data)
        {
            try
            {
                await AssignmentService.GradeAssignmentAsync(data);
               
                var assignment = assignments.First(x => x.Id == data.AssignmentId);
                var studentAssignment = assignment.Students.First(x => x.StudentId == data.StudentId);
                studentAssignment.HasBeenGraded = true;
            }
            catch (Exception)
            {
                ToastService.ShowToast(ToastLevel.Error, "The grade could not be saved...");
            }
        }

        private async Task DeleteAssignmentHandler(Guid assignmentId)
        {
            try
            {
                await AssignmentService.DeleteAsync(assignmentId);
                assignments = assignments.Where(x => x.Id != assignmentId).ToList();
            }
            catch (Exception)
            {
                ToastService.ShowToast(ToastLevel.Error, "The assignment could not be deleted...");
            }
        }

        private void ViewSubmittedWork((Guid assignmentId, Guid studyGroupId) data)
        {
            NavigationManager.NavigateTo($"/{CourseTitle}/{CourseId}/{data.studyGroupId}/{data.assignmentId}/assignment-details");
        }
    }
}
