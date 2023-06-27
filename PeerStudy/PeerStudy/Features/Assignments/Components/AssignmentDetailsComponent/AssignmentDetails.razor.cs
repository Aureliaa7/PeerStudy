using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Services.Interfaces;
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

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private IConfiguration Configuration { get; set; }


        [Parameter]
        public Guid StudyGroupId { get; set; }

        [Parameter]
        public Guid AssignmentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        private AssignmentFilesModel assignmentDetails;
        private bool isLoading;
        private bool showUploadFileDialog; 
        private bool showDeleteFileButton;
        private bool showUploadFilesButton;
        private bool isUploadingFilesInProgress;
        private bool isReadOnly;
        private List<UploadFileModel> allFiles = new List<UploadFileModel>();
        private List<UploadFileModel> newlyAddedFiles = new List<UploadFileModel>();
        private Guid currentUserId;
        private bool canPostponeDeadline;
        private bool isPostponeDeadlineButtonDisabled;

        private const string buttonStyles = "margin: 10px;";

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
                assignmentDetails = await AssignmentFileService.GetUploadedFilesByStudyGroupAsync(AssignmentId, StudyGroupId);
                canPostponeDeadline = await AssignmentService.CanPostponeDeadlineAsync(StudyGroupId, AssignmentId);
                isReadOnly = (await CourseService.GetCourseStatusAsync(CourseId)) == CourseStatus.Archived;
                if (assignmentDetails.CompletedAt == null)
                {
                    showUploadFilesButton = true;
                }
            }
            catch(Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
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

        private void DisplayUploadFilesDialog()
        {
            showUploadFileDialog = true;
        }

        private async Task Unsubmit()
        {
            try
            {
                await AssignmentService.ResetSubmitDateAsync(AssignmentId);

                allFiles.Clear();
                assignmentDetails.CompletedAt = null;
                foreach (var file in assignmentDetails.StudyGroupAssignmentFiles)
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while unsubmitting your work...");
            }

            StateHasChanged();
        }

        private async Task DeleteFile(string fileName)
        {
            showUploadFilesButton = true;
            allFiles = allFiles.Where(x => x.Name != fileName).ToList();
            newlyAddedFiles = newlyAddedFiles.Where(x => x.Name != fileName).ToList();
            ToastService.ShowToast(ToastLevel.Info, "Deleting file...", false);

            if (assignmentDetails.StudyGroupAssignmentFiles != null && 
                assignmentDetails.StudyGroupAssignmentFiles.Any(x => x.Name == fileName))
            {
                try
                {
                    var toBeDeleted = assignmentDetails.StudyGroupAssignmentFiles.FirstOrDefault(x => x.Name == fileName);
                    await AssignmentFileService.DeleteAsync(toBeDeleted.FileDriveId, toBeDeleted.Id);
                    assignmentDetails.StudyGroupAssignmentFiles.Remove(toBeDeleted);
                    ToastService.ClearAll(ToastLevel.Info);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, "The file could not be deleted...");
                }
            }
        }

        private async Task Submit()
        {
            isUploadingFilesInProgress = true;
            showDeleteFileButton = false;
            showUploadFilesButton = false;
            ToastService.ShowToast(ToastLevel.Info, "Uploading file(s)...", false);

            DateTime completedAt = DateTime.UtcNow;
            try
            {
                await Task.Run(async () =>
                {
                    var createdResources = await AssignmentFileService.UploadWorkAsync(new UploadAssignmentFilesModel
                    {
                        AssignmentId = AssignmentId,
                        StudyGroupId = StudyGroupId,
                        Files = newlyAddedFiles,
                        OwnerId = currentUserId
                    }, completedAt);
                    assignmentDetails.StudyGroupAssignmentFiles.AddRange(createdResources);
                    assignmentDetails.CompletedAt = completedAt;
                    allFiles.Clear();
                });

                ToastService.ShowToast(ToastLevel.Success, "Files were successfully uploaded.");
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "There was an error while uploading the files...");
            }

            newlyAddedFiles.Clear();
            isUploadingFilesInProgress = false;
            StateHasChanged();
        }

        private bool ShowUnsubmitButton()
        {
            return assignmentDetails.CompletedAt != null &&
                assignmentDetails.StudyGroupAssignmentFiles != null &&
                assignmentDetails.StudyGroupAssignmentFiles.Any() &&
                !allFiles.Any();
        }

        private async Task PostponeDeadline()
        {
            isPostponeDeadlineButtonDisabled = true;
            StateHasChanged();

            try
            {
                assignmentDetails.Deadline = await AssignmentService.PostponeDeadlineAsync(currentUserId, AssignmentId, StudyGroupId);
                canPostponeDeadline = await AssignmentService.CanPostponeDeadlineAsync(StudyGroupId, AssignmentId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
                canPostponeDeadline = false;
            }

            isPostponeDeadlineButtonDisabled = !canPostponeDeadline;
        }
    }
}
