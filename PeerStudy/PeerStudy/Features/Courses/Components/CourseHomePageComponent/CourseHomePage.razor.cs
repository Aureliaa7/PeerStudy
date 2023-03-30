using Blazored.Toast.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Features.Courses.Store;
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
        private bool isReadOnly;
        private bool showAddAssigmentDialog;
        private int[] studyGroupsNoMembers = new int[3] { 3, 4, 5 };  //TODO: should be moved to constants file

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
            SetCurrentCourseDetails();
            UpdateNavigationMenu();
            isReadOnly = courseDetails.Status == CourseStatus.Archived;
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
            ToastService.ShowToast(ToastLevel.Info, "Uploading file(s)...", false);
            CloseUploadFileDialog();

            await Task.Run(async () =>
            {
                var uploadFileModels = GetCreateResourceModels(filesModels);
                var createdResources = await CourseResourceService.UploadResourcesAsync(uploadFileModels);
                resources.AddRange(createdResources);
            });
            ToastService.ShowToast(ToastLevel.Success, "Files were successfully uploaded.");

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
            ToastService.ShowToast(ToastLevel.Info, "Deleting resource...", false);

            try
            {
                await CourseResourceService.DeleteAsync(resourceId);
                resources = resources.Where(x => x.Id != resourceId).ToList();
                ToastService.ShowToast(ToastLevel.Success, "The resource was successfully deleted.");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, deleteResourceErrorMessage);
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

        private void ShowCreateAssignmentDialog()
        {
            showCreateMenu = false;
            showAddAssigmentDialog = true;
        }

        private async Task SaveAssignment()
        {
            showAddAssigmentDialog = false;
            ToastService.ShowToast(ToastLevel.Info, "Adding assignment...", false);
            assignmentModel.CourseId = CourseId;
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
            }

            StateHasChanged();

            assignmentModel = new CreateAssignmentModel();
        }

        private void CancelCreateAssignment()
        {
            showAddAssigmentDialog = false;
            assignmentModel = new CreateAssignmentModel();
        }
    }
}
