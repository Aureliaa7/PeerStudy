using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.StudyGroups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.CourseStudyGroupsComponent
{
    public partial class CourseStudyGroups : StudyGroupsBase
    {
        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }


        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        private List<StudyGroupDetailsModel> studyGroups = new List<StudyGroupDetailsModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                studyGroups = await StudyGroupService.GetByCourseIdAsync(CourseId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while fetching the study groups...");
            }
        }
    }
}
