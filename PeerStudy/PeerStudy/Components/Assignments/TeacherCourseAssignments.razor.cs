using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Assignments
{
    public partial class TeacherCourseAssignments: PeerStudyComponentBase<ExtendedAssignmentDetailsModel>
    {
        [Inject]
        public IAssignmentService AssignmentService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICourseService CourseService { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private const string noAssignmentsMessage = "There are no assignments yet...";
        private const int snackBarTimeout = 6000;

        private bool showMessage;
        private bool isReadOnly;
        private string message;

        protected override async Task OnInitializedAsync()
        {
            isReadOnly = (await CourseService.GetCourseStatusAsync(CourseId)) == CourseStatus.Archived;
            await InitializeDataAsync();
        }

        protected override Task<List<ExtendedAssignmentDetailsModel>> GetDataAsync()
        {
            return AssignmentService.GetByCourseIdAsync(CourseId);
        }

        private async void SaveGradeHandler(SaveGradeModel data)
        {
            try
            {
                await AssignmentService.GradeAssignmentAsync(data);
            }
            catch (Exception)
            {
                DisplayErrorMessage("The grade could not be saved...");
            }
        }

        private async Task DeleteAssignmentHandler(Guid assignmentId)
        {
            try
            {
                await AssignmentService.DeleteAsync(assignmentId);
                data = data.Where(x => x.AssignmentId != assignmentId).ToList();
            }
            catch (Exception)
            {
                DisplayErrorMessage("The assignment could not be deleted...");
            }
        }

        private void ViewSubmittedWork((Guid assignmentId, Guid studentId) data)
        {
            NavigationManager.NavigateTo($"/{CourseTitle}/{CourseId}/{data.studentId}/{data.assignmentId}/assignment-details");
        }

        private void DisplayErrorMessage(string errorMessage)
        {
            message = errorMessage;
            showMessage = true;
        }
    }
}
