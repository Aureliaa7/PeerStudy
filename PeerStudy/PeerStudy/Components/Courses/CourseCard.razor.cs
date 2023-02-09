using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CourseCard
    {
        [Parameter]
        public bool CanEnrollInCourse { get; set; }
        
        [Parameter]
        public CourseDetailsModel Course { get; set; }

        [Parameter]
        public EventCallback<CourseDetailsModel> OnEditCourse { get; set; }

        [Parameter]
        public EventCallback<Guid> OnArchiveCourse { get; set; }
        
        [Parameter]
        public EventCallback<CourseDetailsModel> OnCourseClicked { get; set; }

        [Parameter]
        public EventCallback<CourseDetailsModel> OnEnroll { get; set; }

        private string cardStyles = "width: 80%; height: 90%;";

        public async Task EditCourse()
        {
            await OnEditCourse.InvokeAsync(Course);
        }

        private async Task ArchiveCourse()
        {
            await OnArchiveCourse.InvokeAsync(Course.Id);
        }

        private async Task ClickedCourseHandler()
        {
            await OnCourseClicked.InvokeAsync(Course);
        }

        private async Task Enroll()
        {
            await OnEnroll.InvokeAsync(Course);
        }
    }
}
