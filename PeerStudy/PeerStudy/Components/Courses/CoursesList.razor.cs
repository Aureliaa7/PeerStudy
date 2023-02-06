using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CoursesList
    {
        [Parameter]
        public List<CourseDetailsModel> Courses { get; set; } = new();

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public string CoursesNotFoundMessage { get; set; }

        [Parameter]
        public EventCallback<CourseDetailsModel> OnEditCourse { get; set; }

        [Parameter]
        public EventCallback<Guid> OnArchiveCourse { get; set; }

        [Parameter]
        public EventCallback<CourseDetailsModel> OnCourseClicked { get; set; }

        [Parameter]
        public EventCallback<CourseDetailsModel> OnEnroll { get; set; }

        private async Task EditCourseHandler(CourseDetailsModel course)
        {
            await OnEditCourse.InvokeAsync(course);
        }

        private async Task ArchiveCourseHandler(Guid courseId)
        {
            await OnArchiveCourse.InvokeAsync(courseId);
        }

        private async Task ClickedCourseHandler(CourseDetailsModel course)
        {
            await OnCourseClicked.InvokeAsync(course);
        }

        private async Task EnrollHandler(CourseDetailsModel course)
        {
            await OnEnroll.InvokeAsync(course);
        }
    }
}
