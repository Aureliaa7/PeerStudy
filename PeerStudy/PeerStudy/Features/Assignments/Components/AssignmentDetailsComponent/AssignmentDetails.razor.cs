using Blazorise;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.AssignmentDetailsComponent
{
    public partial class AssignmentDetails
    {
        [Inject]
        private IAssignmentFileService AssignmentFileService { get; set; }

        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private ICourseService CourseService { get; set; }


        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid AssignmentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        private AssignmentFilesModel assignmentDetails;
        private bool isLoading;
        private bool showUploadFileDialog; 
        private bool showAlertMessage;
        private bool showDeleteFileButton;
        private bool showUploadFilesButton;
        private bool isUploadingFilesInProgress;
        private bool isReadOnly;
        private string alertMessage;
        private Color alertColor;
        private List<UploadFileModel> allFiles = new List<UploadFileModel>();
        private List<UploadFileModel> newlyAddedFiles = new List<UploadFileModel>();

        private const string noFilesMessage = "There are no files yet...";
        private const string buttonStyles = "margin: 10px;";

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            assignmentDetails = await AssignmentFileService.GetUploadedFilesByStudentAsync(AssignmentId, StudentId);
            isReadOnly = (await CourseService.GetCourseStatusAsync(CourseId)) == CourseStatus.Archived;
            if (assignmentDetails.CompletedAt == null)
            {
                showUploadFilesButton = true;
            }
            isLoading = false;
        }

        private void CloseUploadFilesDialog()
        {
            showUploadFileDialog = false;
        }

        private void Upload(List<UploadFileModel> files)
        {
            showUploadFileDialog = false;
            newlyAddedFiles.AddRange(files);
            allFiles.AddRange(files);
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

        private async Task Unsubmit()
        {
            try
            {
                await AssignmentService.ResetSubmitDateAsync(assignmentDetails.StudentAssignmentId);

                allFiles.Clear();
                assignmentDetails.CompletedAt = null;
                foreach (var file in assignmentDetails.StudentAssignmentFiles)
                {
                    allFiles.Add(new UploadFileModel
                    {
                        Name = file.Name
                    });
                }

                showDeleteFileButton = true;
                showUploadFilesButton = true;
            }
            catch(Exception ex)
            {
                DisplayAlert(Color.Danger, "An error occurred while unsubmitting your work...");
            }

            StateHasChanged();
        }

        private async Task DeleteFile(string fileName)
        {
            showUploadFilesButton = true;
            allFiles = allFiles.Where(x => x.Name != fileName).ToList();
            newlyAddedFiles = newlyAddedFiles.Where(x => x.Name != fileName).ToList();
            DisplayAlert(Color.Info, "Deleting file...");

            if (assignmentDetails.StudentAssignmentFiles != null && 
                assignmentDetails.StudentAssignmentFiles.Any(x => x.Name == fileName))
            {
                try
                {
                    var toBeDeleted = assignmentDetails.StudentAssignmentFiles.FirstOrDefault(x => x.Name == fileName);
                    await AssignmentFileService.DeleteAsync(toBeDeleted.FileDriveId, toBeDeleted.Id);
                    assignmentDetails.StudentAssignmentFiles.Remove(toBeDeleted);      
                }
                catch (Exception ex)
                {
                    DisplayAlert(Color.Danger, "The file could not be deleted...");
                }
            }
            showAlertMessage = false;
        }

        private async Task Submit()
        {
            isUploadingFilesInProgress = true;
            showDeleteFileButton = false;
            showUploadFilesButton = false;
            DisplayAlert(Color.Info, "Uploading file(s)...");

            DateTime completedAt = DateTime.UtcNow;
            try
            {
                await Task.Run(async () =>
                {
                    var createdResources = await AssignmentFileService.UploadWorkAsync(new UploadAssignmentFilesModel
                    {
                        AssignmentId = AssignmentId,
                        StudentId = StudentId,
                        Files = newlyAddedFiles
                    }, completedAt);
                    assignmentDetails.StudentAssignmentFiles.AddRange(createdResources);
                    assignmentDetails.CompletedAt = completedAt;
                    allFiles.Clear();
                });

                DisplayAlert(Color.Success, "Files were successfully uploaded.");
            }
            catch (Exception ex)
            {
                DisplayAlert(Color.Danger, "There was an error while uploading the files...");
            }
            finally
            {
                newlyAddedFiles.Clear();
                isUploadingFilesInProgress = false;
            }

            StateHasChanged();

            await Task.Run(async () =>
            {
                await Task.Delay(3500);
                showAlertMessage = false;
            });
        }

        private bool ShowUnsubmitButton()
        {
            return assignmentDetails.CompletedAt != null &&
                assignmentDetails.StudentAssignmentFiles != null &&
                assignmentDetails.StudentAssignmentFiles.Any() &&
                !allFiles.Any();
        }
    }
}
