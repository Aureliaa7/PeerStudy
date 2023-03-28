using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudentStudyGroupsComponent
{
    public partial class StudentStudyGroups : StudyGroupsBase
    {
        [Inject]
        public IStudyGroupService StudyGroupService { get; set; }

        [Inject]
        private ICacheService CacheService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await SetCurrentUserDataAsync();

                studyGroups = await CacheService.GetAsync($"{currentUserId}_{ClientConstants.StudentStudyGroupsKey}",
                   () => StudyGroupService.GetByStudentIdAsync(currentUserId));
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Could not fetch the study groups...");
            }
        }
    }
}
