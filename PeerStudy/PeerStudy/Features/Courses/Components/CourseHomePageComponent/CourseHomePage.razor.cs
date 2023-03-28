using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.CourseHomePageComponent
{
    public partial class CourseHomePage : PeerStudyComponentBase
    {
        [Inject]
        private ICourseResourceService CourseResourceService { get; set; }

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }

        [Inject]
        private ICourseService CourseService { get; set; }

        [Inject]
        private IAssignmentService AssignmentService { get; set; }


        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }


        private CreateAssignmentModel assignmentModel = new CreateAssignmentModel();
        private List<ResourceDetailsModel> resources = new List<ResourceDetailsModel>();
        private CourseDetailsModel courseDetails;
        private bool showCreateMenu;
        private bool showUploadFileDialog;
        private bool showCreateStudyGroupsDialog;
        private bool showAlertMessage;
        private bool isReadOnly;
        private bool showAddAssigmentDialog;
        private int[] studyGroupsNoMembers = new int[3] { 3, 4, 5 };  //TODO: should be moved to constants file
        private Color alertColor;
        private string alertMessage;

        private const string menuButtonsStyles = "color: white;";
        private const string deleteResourceErrorMessage = "The resource could not be deleted. Please try again later...";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();

            resources = await CourseResourceService.GetByCourseIdAsync(CourseId);
            await SetCurrentCourseDetailsAsync();
            UpdateNavigationMenu();
            isReadOnly = courseDetails.Status == CourseStatus.Archived;
        }

        private async Task SetCurrentCourseDetailsAsync()
        {
            //TODO: get data from store
            courseDetails = await CourseService.GetDetailsAsync(CourseId);
        }

        private void UpdateNavigationMenu()
        {
            NavigationMenuService.AddCourseNavigationMenuItems(
                currentUserId,
                CourseId,
                courseDetails.Title,
                currentUserRole);
        }

        private void ToggleShowCreateMenu()
        {
            showCreateMenu = !showCreateMenu;
        }

        private void ShowUploadFileDialog()
        {
            showCreateMenu = false;
            showUploadFileDialog = true;
        }

        private void ShowCreateGroupsDialog()
        {
            showCreateMenu = false;
            showCreateStudyGroupsDialog = true;
        }

        private async Task UploadFiles(List<UploadFileModel> filesModels)
        {
            ShowToast(ToastLevel.Info, "Uploading file(s)...", false);
            CloseUploadFileDialog();

            await Task.Run(async () =>
            {
                var uploadFileModels = GetCreateResourceModels(filesModels);
                var createdResources = await CourseResourceService.UploadResourcesAsync(uploadFileModels);
                resources.AddRange(createdResources);
            });
            ShowToast(ToastLevel.Success, "Files were successfully uploaded.");

            //go back on the main thread
            StateHasChanged();
        }

        private UploadCourseResourcesModel GetCreateResourceModels(List<UploadFileModel> files)
        {
            var uploadFileModels = new List<UploadDriveFileModel>();

            foreach (var file in files)
            {
                uploadFileModels.Add(new UploadDriveFileModel
                {
                    FileContent = file.FileContent,
                    Name = file.Name,
                    OwnerEmail = userEmail,
                    // Type = file.Type, //TODO: check why file.type was used
                });
            }

            return new UploadCourseResourcesModel
            {
                CourseId = CourseId,
                OwnerId = currentUserId,
                Resources = uploadFileModels
            };
        }

        private void CloseUploadFileDialog()
        {
            showUploadFileDialog = false;
        }

        private async Task DeleteResource(Guid resourceId)
        {
            ShowToast(ToastLevel.Info, "Deleting resource...", false);

            try
            {
                await CourseResourceService.DeleteAsync(resourceId);
                showAlertMessage = false;
                resources = resources.Where(x => x.Id != resourceId).ToList();
                ShowToast(ToastLevel.Success, "The resource was successfully deleted.");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ShowToast(ToastLevel.Error, deleteResourceErrorMessage);
            }
        }

        private void CancelCreateStudyGroups()
        {
            showCreateStudyGroupsDialog = false;
        }

        private async Task CreateGroups(string noStudentsPerGroup)
        {
            showCreateStudyGroupsDialog = false;
            ShowToast(ToastLevel.Info, "Creating study groups...", false);

            try
            {
                await StudyGroupService.CreateStudyGroupsAsync(currentUserId, CourseId, Convert.ToInt16(noStudentsPerGroup));
                ShowToast(ToastLevel.Success, "Study groups were successfully created.");
            }
            catch (Exception ex)
            {
                ShowToast(ToastLevel.Error, ex.Message);
            }
        }

        private void ShowCreateAssignmentDialog()
        {
            showCreateMenu = false;
            showAddAssigmentDialog = true;
        }

        private async Task SaveAssignment()
        {
            showAddAssigmentDialog = false;
            ShowToast(ToastLevel.Info, "Adding assignment...", false);
            assignmentModel.CourseId = CourseId;
            assignmentModel.TeacherId = currentUserId;
            assignmentModel.DueDate = assignmentModel.DueDate.AddDays(1); // fix for MatDatePicker

            try
            {
                await Task.Run(async () =>
                {
                    await AssignmentService.CreateAsync(assignmentModel);
                });
                ShowToast(ToastLevel.Success, "Assignment was successfully added.");
            }
            catch (Exception ex)
            {
                ShowToast(ToastLevel.Error, "An error occurred. Please try again later.");
            }

            StateHasChanged();

            assignmentModel = new CreateAssignmentModel();  // after saving the assignment, reset the form
        }

        private void CancelCreateAssignment()
        {
            showAddAssigmentDialog = false;
            assignmentModel = new CreateAssignmentModel();
        }
    }
}
