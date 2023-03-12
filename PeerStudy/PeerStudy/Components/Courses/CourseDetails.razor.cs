using Blazorise;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CourseDetails : PeerStudyComponentBase<CourseResourceDetailsModel>
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

        private bool showCreateMenu;
        private bool showUploadFileDialog;

        private const string menuButtonsStyles = "color: white;";

        private string alertMessage;
        private bool showAlertMessage;

        private string userEmail;
        private Color alertColor;

        private bool showCreateStudyGroupsDialog;
        private bool isReadOnly;

        private const string deleteResourceErrorMessage = "The resource could not be deleted. Please try again later...";
        private int[] studyGroupsNoMembers = new int[3] { 3, 4, 5 };  //TODO: should be moved to constants file

        private CourseDetailsModel courseDetails;

        // assignments
        private CreateAssignmentModel assignmentModel = new CreateAssignmentModel();
        private bool showAddAssigmentDialog;

        protected override Task<List<CourseResourceDetailsModel>> GetDataAsync()
        {
            return CourseResourceService.GetAsync(CourseId);
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
            await SetCurrentCourseDetailsAsync();
            isReadOnly = courseDetails.Status == CourseStatus.Archived;
            UpdateNavigationMenu();

            userEmail = await AuthService.GetCurrentUserEmailAsync();
        }

        private async Task SetCurrentCourseDetailsAsync()
        {
            courseDetails = await CourseService.GetDetailsAsync(CourseId);
        }

        private void UpdateNavigationMenu()
        {
            var navigationData = new NavigationDataModel
            {
                CourseId = CourseId,
                CourseTitle = courseDetails.Title,
                UserId = currentUserId
            };

            if (isTeacher)
            {
                NavigationMenuService.AddNavigationMenuItemsForTeacher(navigationData);
            }
            else if (isStudent)
            {
                NavigationMenuService.AddNavigationMenuItemsForStudent(navigationData);
            }
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
            ShowAlert(Color.Info, "Uploading file(s)...");

            CloseUploadFileDialog();

            await Task.Run(async () => 
            {
                var uploadFileModels = GetCreateResourceModels(filesModels);
                var createdResources = await CourseResourceService.UploadResourcesAsync(uploadFileModels); 
                data.AddRange(createdResources);
            });

            ShowAlert(Color.Success, "Files were successfully uploaded.");

            //go back on the main thread
            StateHasChanged();

            await Task.Run(async () =>
            {
                await Task.Delay(3500);
                showAlertMessage = false;
            });
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
                Resources = uploadFileModels
            };
        }

        private void CloseUploadFileDialog()
        {
            showUploadFileDialog = false;
        }

        private async Task DeleteResource(Guid resourceId)
        {
            ShowAlert(Color.Info, "Deleting resource...");

            try
            {
                await CourseResourceService.DeleteAsync(resourceId);
                showAlertMessage = false;
                data = data.Where(x => x.Id != resourceId).ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ShowAlert(Color.Danger, deleteResourceErrorMessage);
            }
        }

        private void CancelCreateStudyGroups()
        {
            showCreateStudyGroupsDialog = false;
        }

        private async Task CreateGroups(string noStudentsPerGroup)
        {
            showCreateStudyGroupsDialog = false;
            ShowAlert(Color.Info, "Creating study groups...");

            try
            {
                await StudyGroupService.CreateStudyGroupsAsync(currentUserId, CourseId, Convert.ToInt16(noStudentsPerGroup));
                ShowAlert(Color.Success, "Study groups were successfully created.");
            }
            catch (Exception ex)
            {
                ShowAlert(Color.Danger, ex.Message);
            }
        }

        private void ShowAlert(Color alertColor, string message)
        {
            this.alertColor = alertColor;
            this.alertMessage = message;

            showAlertMessage = true;
        }

        private void ShowCreateAssignmentDialog()
        {
            showCreateMenu = false;
            showAddAssigmentDialog = true;
        }

        private async Task SaveAssignment()
        {
            showAddAssigmentDialog = false;
            ShowAlert(Color.Info, "Adding assignment...");
            assignmentModel.CourseId = CourseId;
            assignmentModel.TeacherId = currentUserId;
            assignmentModel.DueDate = assignmentModel.DueDate.AddDays(1); // fix for MatDatePicker

            try
            {
                await Task.Run(async () =>
                {
                    await AssignmentService.CreateAsync(assignmentModel);
                });
                ShowAlert(Color.Success, "Assignment was successfully added.");
            }
            catch (Exception ex)
            {
                ShowAlert(Color.Danger, "An error occurred. Please try again later.");
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
