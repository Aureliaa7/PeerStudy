using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CourseCard
    {
        [Parameter]
        public CourseDetailsModel Course { get; set; }

        [Parameter]
        public EventCallback<Guid> OnEditCourse { get; set; }

        [Parameter]
        public EventCallback<Guid> OnArchiveCourse { get; set; }


        private string cardStyles = "width: 80%; height: 90%;";

        public async Task EditCourse()
        {
            await OnEditCourse.InvokeAsync(Course.Id);
        }

        private async Task ArchiveCourse()
        {
            await OnArchiveCourse.InvokeAsync(Course.Id);
        }
    }
}
