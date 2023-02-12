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
        
        private string uploadFilesMessage;
        private bool showUploadFilesMessage;

        private IMatFileUploadEntry[] uploadedFiles;
        private string userEmail;
        private Color alertColor;

        protected override Task<List<CourseResourceDetailsModel>> GetDataAsync()
        {
            return CourseResourceService.GetAsync(CourseId);
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();

            userEmail = await AuthService.GetCurrentUserEmailAsync();

            //TODO: to be implemented
            //TODO: update additional nav items depending on the user's role
            NavigationMenuService.AddMenuItems(new List<MenuItem> {
                  new MenuItem
                    {
                        Href = "#",
                        Name = "Resources"
                    },
                    new MenuItem
                    {
                        Href = "#",
                        Name = "Students"
                    },
                    new MenuItem
                    {
                        Href = "#",
                        Name = "Study groups"
                    },
                    new MenuItem
                    {
                        Href = $"/{TeacherId}/courses/{CourseId}/pending-requests",
                        Name = "Pending requests"
                    },
                    new MenuItem
                    {
                        Href = $"/{TeacherId}/courses/{CourseId}/rejected-requests",
                        Name = "Rejected requests"
                    }
                });
            NavigationMenuService.NotifyChanged();
        }

        private void ToggleShowCreateMenu()
        {
            showCreateMenu = !showCreateMenu;
        }

        private void GetUploadedFiles(IMatFileUploadEntry[] files)
        {
            uploadedFiles = files;
        }

        private async Task UploadFiles()
        {
            showCreateMenu = false;
            alertColor = Color.Info;
            uploadFilesMessage = "Uploading file(s)...";
            showUploadFilesMessage = true;

            CloseUploadFileDialog();

            await Task.Run(async () => 
            {
                var uploadFileModels = await GetCreateResourceModelsAsync();
                await CourseResourceService.UploadResourcesAsync(uploadFileModels); 
            });

            uploadedFiles = null;
            uploadFilesMessage = "Files were successfully uploaded.";
            alertColor = Color.Success;

            //go back on the main thread
            StateHasChanged();

            await Task.Run(async () =>
            {
                await Task.Delay(3500);
                showUploadFilesMessage = false;
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
    }
}
