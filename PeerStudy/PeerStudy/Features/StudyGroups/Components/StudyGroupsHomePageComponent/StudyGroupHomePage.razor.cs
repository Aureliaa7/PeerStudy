using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Resources;
using PeerStudy.Core.Models.StudyGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupsHomePageComponent
{
    public partial class StudyGroupHomePage : PeerStudyComponentBase
    {
        [Inject]
        private IStudyGroupResourceService StudyGroupResourceService { get; set; }

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }


        [Parameter]
        public Guid StudyGroupId { get; set; }

        [Parameter]
        public string GroupName { get; set; }


        private List<ResourceDetailsModel> resources = new List<ResourceDetailsModel>();
        private bool isReadOnly;
        private bool showUploadFileDialog;
        private StudyGroupDetailsModel studyGroup;


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            NavigationMenuService.Reset();
            await SetStudyGroupDetailsAsync();
            NavigationMenuService.AddStudyGroupNavigationMenuItems(StudyGroupId, studyGroup.Title);
            await SetCurrentUserDataAsync();
            resources = await StudyGroupResourceService.GetByStudyGroupIdAsync(StudyGroupId);
        }

        private async Task SetStudyGroupDetailsAsync()
        {
            studyGroup = await StudyGroupService.GetAsync(StudyGroupId);
            isReadOnly = !studyGroup.IsActive;
        }

        private async Task UploadFiles(List<UploadFileModel> files)
        {
            showUploadFileDialog = false;
            ToastService.ShowToast(ToastLevel.Info, "Uploading files...", false);

            await Task.Run(async () =>
            {
                var uploadFileModels = GetCreateResourceModels(files);
                var createdResources = await StudyGroupResourceService.UploadAsync(uploadFileModels);
                resources.AddRange(createdResources);
            });

            ToastService.ShowToast(ToastLevel.Success, "Files were successfully uploaded.");
            StateHasChanged();
        }

        private UploadStudyGroupResourceModel GetCreateResourceModels(List<UploadFileModel> files)
        {
            var uploadFileModels = new List<UploadDriveFileModel>();

            foreach (var file in files)
            {
                uploadFileModels.Add(new UploadDriveFileModel
                {
                    FileContent = file.FileContent,
                    Name = file.Name,
                    OwnerEmail = userEmail
                });
            }

            return new UploadStudyGroupResourceModel
            {
                StudyGroupId = StudyGroupId,
                OwnerId = currentUserId,
                Resources = uploadFileModels
            };
        }

        private async Task DeleteResource(Guid id)
        {
            ToastService.ShowToast(ToastLevel.Info, "Deleting resource...", false);

            try
            {
                await StudyGroupResourceService.DeleteAsync(id);
                resources = resources.Where(x => x.Id != id).ToList();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while deleting the resource...");
            }
        }

        // fix for https://github.com/mrpmorris/Fluxor/blob/master/Docs/disposable-callback-not-disposed.md
        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);
            NavigationMenuService.Reset();
        }
    }
}
