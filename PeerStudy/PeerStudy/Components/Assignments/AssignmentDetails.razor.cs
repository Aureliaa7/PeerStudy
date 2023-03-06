using Blazorise;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Assignments
{
    public partial class AssignmentDetails
    {
        [Inject]
        public IAssignmentFileService AssignmentFileService { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid AssignmentId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private AssignmentFilesModel assignmentDetails;
        private bool isLoading;
        private bool showUploadFileDialog; 
        private bool showAlertMessage;
        private string alertMessage;
        private Color alertColor;

        private const string noFilesMessage = "There are no files yet...";

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            assignmentDetails = await AssignmentFileService.GetUploadedFilesByStudentAsync(AssignmentId, StudentId);
            isLoading = false;
        }

        private void CloseUploadFilesDialog()
        {
            showUploadFileDialog = false;
        }

        private async void Upload(List<UploadFileModel> files)
        {
            // create models and call method from service
            showUploadFileDialog = false;
            DisplayAlert(Color.Info, "Uploading file(s)...");

            await Task.Run(async () =>
            {
                var createdResources = await AssignmentFileService.UploadWorkAsync(new UploadAssignmentFilesModel
                {
                    AssignmentId = AssignmentId,
                    StudentId = StudentId,
                    Files = files
                });
                assignmentDetails.StudentAssignmentFiles.AddRange(createdResources);
            });

            DisplayAlert(Color.Success, "Files were successfully uploaded.");

            StateHasChanged();

            await Task.Run(async () =>
            {
                await Task.Delay(3500);
                showAlertMessage = false;
            });
        }

        private void DisplayAlert(Color alertColor, string message)
        {
            this.alertColor = alertColor;
            alertMessage = message;

            showAlertMessage = true;
        }

        private void DisplayUploadFilesDialog()
        {
            showUploadFileDialog = true;
        }

        private void Unsubmit()
        {
            //TODO
        }

        private void DeleteFile()
        {
            //TODO
        }
    }
}
