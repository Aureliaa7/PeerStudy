using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.CourseHomePageComponent
{
    public partial class CourseHomePage : PeerStudyComponentBase, IDisposable
    {
        [Inject]
        private ICourseResourceService CourseResourceService { get; set; }

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }

        [Inject]
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private ICourseService CourseService { get; set; }

        [Inject]
        private ICourseUnitService CourseUnitService { get; set;}

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IStudentPointsService StudentAssetService { get; set; }


        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }


        private CreateAssignmentModel assignmentModel = new CreateAssignmentModel();
        private List<CourseUnitDetailsModel> courseUnits = new List<CourseUnitDetailsModel>();
        private CourseDetailsModel courseDetails;
        private bool showCreateMenu;
        private bool showUploadFileDialog;
        private bool showCreateStudyGroupsDialog;
        private bool isReadOnly;
        private bool showAddAssigmentDialog;
        private int[] studyGroupsNoMembers = new int[3] { 3, 4, 5 };  //TODO: should be moved to constants file

        private bool showCourseUnitDialog;
        private CourseUnitModel courseUnitModel = new CourseUnitModel();
        private Guid? selectedCourseUnitId;
        private bool isEditCourseUnitEnabled;

        private const string menuButtonsStyles = "color: white;";

        private List<DropDownItem> studyGroupsDropdownItems = new List<DropDownItem>();

        private bool showUnlockCourseUnitDialog;
        private bool isConfirmUnlockUnitButtonDisabled;
        private string unlockCourseUnitMessage;
        private const string unlockCourseUnitPopupTitle = "Unlock course unit";
        private int numberOfPoints;

        private bool isDeleteCourseUnitPopupVisible;
        private const string deleteCourseUnitPopupTitle = "Delete Course Unit";

        private bool isDeleteResourcePopupVisible;
        private const string deleteResourcePopupTitle = "Delete Course Unit Resource";
        private DeleteCourseUnitResourceModel? deleteResourceModel;

        private const string addCourseUnitDialogTitle = "Create course unit";
        private const string updateCourseUnitDialogTitle = "Update course unit";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            if (currentUserRole == Role.Student)
            {
                numberOfPoints = await StudentAssetService.GetNoPointsAsync(StudentId);
                courseUnits = await CourseUnitService.GetByCourseAndStudentIdAsync(CourseId, StudentId);
            }
            else if (currentUserRole == Role.Teacher)
            {
                courseUnits = await CourseUnitService.GetByCourseIdAsync(CourseId);
            }

            await SetCurrentCourseDetailsAsync();
            UpdateNavigationMenu();
            isReadOnly = courseDetails.Status == CourseStatus.Archived;
            studyGroupsDropdownItems = (await StudyGroupService.GetStudyGroupIdNamePairsAsync(CourseId))
                .Select(x => new DropDownItem
                {
                    Key = x.Key.ToString(),
                    Value = x.Value
                })
                .ToList();
        }

        private async Task SetCurrentCourseDetailsAsync()
        {
            try
            {
                courseDetails = await CourseService.GetDetailsAsync(CourseId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occured while fetching the course details...");
            }
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

        private void ShowUploadFileDialog(Guid courseUnitId)
        {
            selectedCourseUnitId = courseUnitId;
            showUploadFileDialog = true;
        }

        private void ShowCreateGroupsDialog()
        {
            showCreateMenu = false;
            showCreateStudyGroupsDialog = true;
        }

        private async Task UploadFiles(List<UploadFileModel> filesModels)
        {
            ToastService.ShowToast(ToastLevel.Info, "Uploading file(s)...", false);
            CloseUploadFileDialog();
            var createdResources = new List<CourseResourceDetailsModel>();

            await Task.Run(async () =>
            {
                var uploadFileModels = GetCreateResourceModels(filesModels, selectedCourseUnitId.Value);
                createdResources = await CourseResourceService.UploadResourcesAsync(uploadFileModels);
            });

            //go back on the main thread
            StateHasChanged();
            var courseUnit = courseUnits.FirstOrDefault(x => x.Id == selectedCourseUnitId.Value);
            selectedCourseUnitId = null;
            courseUnit?.Resources.AddRange(createdResources);

            ToastService.ShowToast(ToastLevel.Success, "Files were successfully uploaded.");
        }

        private UploadCourseResourcesModel GetCreateResourceModels(List<UploadFileModel> files, Guid courseUnitId)
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
                Resources = uploadFileModels,
                CourseUnitId = courseUnitId
            };
        }

        private void CloseUploadFileDialog()
        {
            showUploadFileDialog = false;
        }

        private async Task DeleteResource()
        {
            isDeleteResourcePopupVisible = false;
            ToastService.ShowToast(ToastLevel.Info, "Deleting resource...", false);

            try
            {
                await CourseResourceService.DeleteAsync(deleteResourceModel.ResourceId);
                var courseUnit = courseUnits.FirstOrDefault(x => x.Id == deleteResourceModel.CourseUnitId);
                var resourceToBeDeleted = courseUnit.Resources.FirstOrDefault(x => x.Id == deleteResourceModel.ResourceId);    
                courseUnit.Resources.Remove(resourceToBeDeleted);
                
                ToastService.ShowToast(ToastLevel.Success, UIMessages.DeleteResourceSuccessMessage);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.DeleteResourceErrorMessage);
            }

            deleteResourceModel = null;
        }

        private async Task DeleteCourseUnit()
        {
            isDeleteCourseUnitPopupVisible = false;

            try
            {
                ToastService.ShowToast(ToastLevel.Info, UIMessages.DeleteCourseUnitMessage, false);
                var courseUnitToBeDeleted = courseUnits.FirstOrDefault(x => x.Id == selectedCourseUnitId.Value);

                await CourseUnitService.DeleteAsync(selectedCourseUnitId.Value);
                courseUnits.Remove(courseUnitToBeDeleted);
                ToastService.ClearAll(ToastLevel.Info);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, ex.Message, clearAll: true);
            }

            selectedCourseUnitId = null;
        }

        private void ShowEditCourseUnitDialog(Guid id)
        {
            isEditCourseUnitEnabled = true;
            var courseUnitToBeUpdated = courseUnits.FirstOrDefault(x => x.Id == id);
            if (courseUnitToBeUpdated != null)
            {
                courseUnitModel = new UpdateCourseUnitModel
                {
                    Id = id,
                    Title = courseUnitToBeUpdated.Title,
                    IsAvailable = courseUnitToBeUpdated.IsAvailable,
                    NoPointsToUnlock = courseUnitToBeUpdated.NoPointsToUnlock
                };
            }

            showCourseUnitDialog = true;
        }

        private async Task UpdateCourseUnit()
        {
            showCourseUnitDialog = false;

            try
            {
                Guid courseUnitId = ((UpdateCourseUnitModel)courseUnitModel).Id;
                await CourseUnitService.RenameAsync(courseUnitId, courseUnitModel.Title);
                var courseUnitToBeUpdated = courseUnits.FirstOrDefault(x => x.Id == courseUnitId);
                courseUnitToBeUpdated.Title = courseUnitModel.Title;
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateCourseUnitErrorMessage);
            }
            finally
            {
                courseUnitModel = new CourseUnitModel();
                isEditCourseUnitEnabled = false;
            }
        }

        private void CancelCreateStudyGroups()
        {
            showCreateStudyGroupsDialog = false;
        }

        private async Task CreateGroups(string noStudentsPerGroup)
        {
            showCreateStudyGroupsDialog = false;
            ToastService.ShowToast(ToastLevel.Info, UIMessages.CreateStudyGroupsMessage, false);

            try
            {
                await StudyGroupService.CreateStudyGroupsAsync(currentUserId, CourseId, Convert.ToInt16(noStudentsPerGroup));
                ToastService.ShowToast(ToastLevel.Success, UIMessages.CreateStudyGroupsSuccessMessage);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, ex.Message);
            }
        }

        private void ShowCreateAssignmentDialog(Guid courseUnitId)
        {
            selectedCourseUnitId = courseUnitId;
            showAddAssigmentDialog = true;
        }

        private async Task SaveAssignment()
        {
            showAddAssigmentDialog = false;
            ToastService.ShowToast(ToastLevel.Info, UIMessages.AddAssignmentMessage, false);
            assignmentModel.CourseUnitId = selectedCourseUnitId.Value;
            assignmentModel.TeacherId = currentUserId;
            assignmentModel.DueDate = assignmentModel.DueDate.AddDays(1); // fix for MatDatePicker

            try
            {
                await Task.Run(async () =>
                {
                    await AssignmentService.CreateAsync(assignmentModel);
                });
                ToastService.ShowToast(ToastLevel.Success, UIMessages.AddAssignmentSuccessMessage);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.AddAssignmentErrorMessage);
                selectedCourseUnitId = null;
            }

            StateHasChanged();

            assignmentModel = new CreateAssignmentModel();
        }

        private void CancelCreateAssignment()
        {
            showAddAssigmentDialog = false;
            assignmentModel = new CreateAssignmentModel();
        }

        private void ShowCourseUnitDialog()
        {
            showCreateMenu = false;
            showCourseUnitDialog = true;
        }

        private void CancelCreateUpdateCourseUnit()
        {
            showCourseUnitDialog = false;
            courseUnitModel = new CourseUnitModel();
        }

        private async Task SaveCourseUnit()
        {
            try
            {
                showCourseUnitDialog = false;
                courseUnitModel.CourseId = CourseId;
                var createdUnit = await CourseUnitService.CreateAsync(courseUnitModel);
                courseUnits.Add(createdUnit);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, ex.Message);
            }
            finally
            {
                courseUnitModel = new CourseUnitModel();
            }
        }

        private void ViewAssignments(Guid courseUnitId)
        {
            NavigationManager.NavigateTo($"/{courseDetails.Title}/{courseDetails.Id}/{courseUnitId}/assignments");
        }

        private async Task DisplayUnlockCourseUnitDialog(Guid courseUnitId)
        {
            string message;

            showUnlockCourseUnitDialog = true;
            selectedCourseUnitId = courseUnitId;
            var courseUnitToBeUnlocked = courseUnits.First(x => x.Id == selectedCourseUnitId);
            bool previousCourseUnitsAreAvailable = false;
            
            try
            {
                previousCourseUnitsAreAvailable = await CourseUnitService.CheckPreviousCourseUnitsAvailabilityAsync(courseUnitToBeUnlocked.Order, currentUserId);
            }
            catch
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
                return;
            }

            if (!previousCourseUnitsAreAvailable)
            {
                unlockCourseUnitMessage = "You cannot skip course units. To unlock this course unit, you need to unlock the previous ones.";
                isConfirmUnlockUnitButtonDisabled = true;
                return;
            }

            if (numberOfPoints >= courseUnitToBeUnlocked.NoPointsToUnlock)
            {
                message = UIMessages.UnlockCourseUnitConfirmationMessage;
                isConfirmUnlockUnitButtonDisabled = false;
            }
            else
            {
                message = $"To unlock this course unit, you need {courseUnitToBeUnlocked.NoPointsToUnlock - numberOfPoints} more points."; ;
                isConfirmUnlockUnitButtonDisabled = true;
            }

            unlockCourseUnitMessage = $"This course unit requires {courseUnitToBeUnlocked.NoPointsToUnlock} points to unlock. You currently have {numberOfPoints} points. \r\n {message}";
        }

        private void CancelUnlockCourseUnit()
        {
            showUnlockCourseUnitDialog = false;
            selectedCourseUnitId = null;
        }

        private async Task UnlockCourseUnit()
        {
            showUnlockCourseUnitDialog = false;
            try
            {
                await CourseUnitService.UnlockAsync(selectedCourseUnitId.Value, StudentId);

                var courseUnitToBeUnlocked = courseUnits.First(x => x.Id == selectedCourseUnitId);
                courseUnitToBeUnlocked.IsAvailable = true;
                numberOfPoints -= courseUnitToBeUnlocked.NoPointsToUnlock;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UnlockCourseUnitErrorMessage);
            }
            selectedCourseUnitId = null;
            unlockCourseUnitMessage = null;
        }

        private void ShowCourseUnitDeleteConfirmationPopup(Guid courseUnitId)
        {
            selectedCourseUnitId = courseUnitId;
            isDeleteCourseUnitPopupVisible = true;
        }

        private void CancelDeleteCourseUnit()
        {
            selectedCourseUnitId = null;
            isDeleteCourseUnitPopupVisible = false;
        }

        private void CancelDeleteResource()
        {
            isDeleteResourcePopupVisible = false;
            deleteResourceModel = null;
        }

        private void ShowDeleteResourcePopup(DeleteCourseUnitResourceModel model)
        {
            isDeleteResourcePopupVisible = true;
            deleteResourceModel = model;
        }

        public void Dispose() => NavigationMenuService.Reset();
    }
}
