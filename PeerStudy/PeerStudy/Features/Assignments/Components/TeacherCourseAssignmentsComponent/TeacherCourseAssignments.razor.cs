using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
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
        private ICourseService CourseService { get; set; }

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
            //TODO: get course status from store
            try
            {
                isReadOnly = (await CourseService.GetCourseStatusAsync(CourseId)) == CourseStatus.Archived;
                await SetCurrentUserDataAsync();
                assignments = await AssignmentService.GetByCourseIdAsync(CourseId);
            }
            catch (Exception ex)
            {
                ToastService.ShowError("An error occurred while fetching the assignments...");
            }
        }

        private async void SaveGradeHandler(SaveGradeModel data)
        {
            try
            {
                await AssignmentService.GradeAssignmentAsync(data);
            }
            catch (Exception)
            {
                ToastService.ShowError("The grade could not be saved...");
            }
        }

        private async Task DeleteAssignmentHandler(Guid assignmentId)
        {
            try
            {
                await AssignmentService.DeleteAsync(assignmentId);
                assignments = assignments.Where(x => x.AssignmentId != assignmentId).ToList();
            }
            catch (Exception)
            {
                ToastService.ShowError("The assignment could not be deleted...");
            }
        }

        private void ViewSubmittedWork((Guid assignmentId, Guid studentId) data)
        {
            NavigationManager.NavigateTo($"/{CourseTitle}/{CourseId}/{data.studentId}/{data.assignmentId}/assignment-details");
        }
    }
}
