using Blazored.Toast.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Features.Courses.Store;
using PeerStudy.Models;
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
        private IAssignmentService AssignmentService { get; set; }

        [Inject]
        private IStateSelection<CoursesState, CourseDetailsModel> SelectedCourse { get; set; }

        [Inject]
        private IState<CoursesState> CoursesState { get; set; }

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
        private const string deleteResourceErrorMessage = "The resource could not be deleted. Please try again later...";

        private List<DropDownItem> studyGroupsDropdownItems = new List<DropDownItem>();

        private bool showUnlockCourseUnitDialog;
        private bool isConfirmUnlockUnitButtonDisabled;
        private string unlockCourseUnitMessage;
        private const string unlockCourseUnitDialogTitle = "Unlock course unit";
        private int numberOfPoints;

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

            SetCurrentCourseDetails();
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

        private void SetCurrentCourseDetails()
        {
            courseDetails = CoursesState.Value.ActiveCourses.FirstOrDefault(x => x.Id == CourseId) ??
                CoursesState.Value.ArchivedCourses.FirstOrDefault(x => x.Id == CourseId);

            SelectedCourse.Select(x => x.ActiveCourses.FirstOrDefault(y => y.Id == CourseId) ??
           x.ArchivedCourses.FirstOrDefault(y => y.Id == CourseId));

            SelectedCourse.SelectedValueChanged += (object? sender, CourseDetailsModel course) =>
            {
                courseDetails = course;
            };
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

        private async Task DeleteResource(DeleteCourseUnitResourceModel courseUnitIdResourceId)
        {
            ToastService.ShowToast(ToastLevel.Info, "Deleting resource...", false);

            try
            {
                await CourseResourceService.DeleteAsync(courseUnitIdResourceId.ResourceId);
                var courseUnit = courseUnits.FirstOrDefault(x => x.Id == courseUnitIdResourceId.CourseUnitId);
                var resourceToBeDeleted = courseUnit.Resources.FirstOrDefault(x => x.Id == courseUnitIdResourceId.ResourceId);    
                courseUnit.Resources.Remove(resourceToBeDeleted);
                
                ToastService.ShowToast(ToastLevel.Success, "The resource was successfully deleted.");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, deleteResourceErrorMessage);
            }
        }

        private async Task DeleteCourseUnit(Guid id)
        {
            try
            {
                ToastService.ShowToast(ToastLevel.Info, "Deleting course unit...", false);
                var courseUnitToBeDeleted = courseUnits.FirstOrDefault(x => x.Id == id);

                await CourseUnitService.DeleteAsync(id);
                courseUnits.Remove(courseUnitToBeDeleted);
                ToastService.ClearAll(ToastLevel.Info);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, ex.Message, clearAll: true);
            }
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred...");
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
            ToastService.ShowToast(ToastLevel.Info, "Creating study groups...", false);

            try
            {
                await StudyGroupService.CreateStudyGroupsAsync(currentUserId, CourseId, Convert.ToInt16(noStudentsPerGroup));
                ToastService.ShowToast(ToastLevel.Success, "Study groups were successfully created.");
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
            ToastService.ShowToast(ToastLevel.Info, "Adding assignment...", false);
            assignmentModel.CourseUnitId = selectedCourseUnitId.Value;
            assignmentModel.TeacherId = currentUserId;
            assignmentModel.DueDate = assignmentModel.DueDate.AddDays(1); // fix for MatDatePicker

            try
            {
                await Task.Run(async () =>
                {
                    await AssignmentService.CreateAsync(assignmentModel);
                });
                ToastService.ShowToast(ToastLevel.Success, "Assignment was successfully added.");
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred. Please try again later.");
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

        private void DisplayUnlockCourseUnitDialog(Guid courseUnitId)
        {
            showUnlockCourseUnitDialog = true;
            selectedCourseUnitId = courseUnitId;
            var courseUnitToBeUnlocked = courseUnits.First(x => x.Id == selectedCourseUnitId);

            string message;

            if (numberOfPoints >= courseUnitToBeUnlocked.NoPointsToUnlock)
            {
                message = "Are you sure you want to unlock this course unit?";
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

        private async void UnlockCourseUnit()
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while unlocking the course...");
            }
            selectedCourseUnitId = null;
            unlockCourseUnitMessage = null;
        }
    }
}
