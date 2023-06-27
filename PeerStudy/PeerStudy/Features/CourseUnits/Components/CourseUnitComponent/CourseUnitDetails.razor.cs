using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.Resources;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseUnits.Components.CourseUnitComponent
{
    public partial class CourseUnitDetails
    {
        [Parameter]
        public CourseUnitDetailsModel CourseUnit { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnRename { get; set; }

        [Parameter]
        public EventCallback<Guid> OnUploadFiles { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddAssignment { get; set; }

        [Parameter]
        public EventCallback<Guid> OnViewAssignments { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClickedLockIcon { get; set; }

        [Parameter]
        public EventCallback<DeleteCourseUnitResourceModel> OnDeleteResource { get; set; }


        private bool showMenuOptions;

        private const string menuButtonStyles = "float:right; height:20px;";
        private const string menuButtonsStyles = "color: white;";
        private const string defaultCursor = "cursor: default;";

        private void ToggleMenuOptions()
        {
            showMenuOptions = !showMenuOptions;
        }

        private void HideMenuOptions()
        {
            showMenuOptions = false;
        }

        private async Task Delete()
        {
            HideMenuOptions();
            await OnDelete.InvokeAsync(CourseUnit.Id);
        }

        private async Task Rename()
        {
            HideMenuOptions();
            await OnRename.InvokeAsync(CourseUnit.Id);
        }

        private async Task UploadFiles()
        {
            HideMenuOptions();
            await OnUploadFiles.InvokeAsync(CourseUnit.Id);
        }

        private async Task DeleteResource(Guid resourceId)
        {
            HideMenuOptions();
            await OnDeleteResource.InvokeAsync(new DeleteCourseUnitResourceModel
            {
                CourseUnitId = CourseUnit.Id,
                ResourceId = resourceId
            });
        }

        private async Task AddAssignment()
        {
            HideMenuOptions();
            await OnAddAssignment.InvokeAsync(CourseUnit.Id);
        }

        private async Task ViewAssignments()
        {
            HideMenuOptions();
            await OnViewAssignments.InvokeAsync(CourseUnit.Id);
        }

        private async Task HandleClickedLockIcon()
        {
            HideMenuOptions();
            await OnClickedLockIcon.InvokeAsync(CourseUnit.Id);
        }
    }
}
