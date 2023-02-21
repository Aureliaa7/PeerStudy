using Blazorise;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CourseDetails : PeerStudyComponentBase<CourseResourceDetailsModel>
    {
        [Inject]
        private INavigationMenuService NavigationMenuService { get; set; }  

        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private ICourseResourceService CourseResourceService { get; set; } 

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }


        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private bool showCreateMenu;
        private bool showUploadFileDialog;

        private const string uploadFileControlStyles = "border-radius: 30px; background-color: #F5F5F5; height: 30px;";
        
        private string alertMessage;
        private bool showAlertMessage;

        private IMatFileUploadEntry[] uploadedFiles;
        private string userEmail;
        private Color alertColor;

        private bool showCreateStudyGroupsDialog;

        private const string deleteResourceErrorMessage = "The resource could not be deleted. Please try again later...";
        private int[] studyGroupsNoMembers = new int[3] { 3, 4, 5 };

        protected override Task<List<CourseResourceDetailsModel>> GetDataAsync()
        {
            return CourseResourceService.GetAsync(CourseId);
        }

        protected override async Task OnInitializedAsync()
        {
            NavigationMenuService.AddMenuItems(new List<MenuItem> {
                  new MenuItem
                    {
                        Href = $"/{TeacherId}/courses/{CourseTitle}/{CourseId}/resources",
                        Name = "Resources"
                    },
                    new MenuItem
                    {
                        Href = $"/courses/{CourseTitle}/{CourseId}/students",
                        Name = "Students"
                    },
                    new MenuItem
                    {
                        Href = $"/{TeacherId}/courses/{CourseTitle}/{CourseId}/pending-requests",
                        Name = "Pending requests"
                    },
                    new MenuItem
                    {
                        Href = $"/{TeacherId}/courses/{CourseTitle}/{CourseId}/rejected-requests",
                        Name = "Rejected requests"
                    }
                });
            NavigationMenuService.NotifyChanged();
            await InitializeDataAsync();

            userEmail = await AuthService.GetCurrentUserEmailAsync();

            //TODO: to be implemented
            //TODO: update additional nav items depending on the user's role
            
        }

        private void ToggleShowCreateMenu()
        {
            showCreateMenu = !showCreateMenu;
        }

        private void GetUploadedFiles(IMatFileUploadEntry[] files)
        {
            uploadedFiles = files;
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

        private async Task UploadFiles()
        {
            ShowAlert(Color.Info, "Uploading file(s)...");

            CloseUploadFileDialog();

            await Task.Run(async () => 
            {
                var uploadFileModels = await GetCreateResourceModelsAsync();
                var createdResources = await CourseResourceService.UploadResourcesAsync(uploadFileModels); 
                data.AddRange(createdResources);
            });

            uploadedFiles = null;

            ShowAlert(Color.Success, "Files were successfully uploaded.");

            //go back on the main thread
            StateHasChanged();

            await Task.Run(async () =>
            {
                await Task.Delay(3500);
                showAlertMessage = false;
            });
        }

        private async Task<List<UploadCourseResourceModel>> GetCreateResourceModelsAsync()
        {
            var uploadFileModels = new List<UploadCourseResourceModel>();

            foreach (var file in uploadedFiles)
            {
                using (var stream = new MemoryStream())
                {
                    await file.WriteToStreamAsync(stream);
                    uploadFileModels.Add(new UploadCourseResourceModel
                    {
                        FileContent = stream.ToArray(),
                        Name = file.Name,
                        OwnerEmail = userEmail,
                        Type = file.Type,
                        CourseId = CourseId
                    });
                }
            }

            return uploadFileModels;
        }

        private void CloseUploadFileDialog()
        {
            showUploadFileDialog = false;
        }

        private bool IsUploadFileButtonEnabled()
        {
            return uploadedFiles != null;
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
    }
}
