using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using System;
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
                ToastService.ShowError("An error occurred while fetching the study groups...");
            }
        }
    }
}
