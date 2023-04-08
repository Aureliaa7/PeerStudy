using Blazored.Toast.Services;
using PeerStudy.Core.Models.StudyGroups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components
{
    public abstract partial class StudentStudyGroupsBase : StudyGroupsBase
    {
        protected List<StudentStudyGroupDetailsModel> studyGroups = new List<StudentStudyGroupDetailsModel>();

        protected abstract Task<List<StudentStudyGroupDetailsModel>> GetStudyGroupsDetailsAsync();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await SetCurrentUserDataAsync();

                studyGroups = await GetStudyGroupsDetailsAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "Could not fetch the study groups...");
            }
        }
    }
}
