using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;

namespace PeerStudy.Features.Courses.Components
{
    public abstract class CoursesBase : PeerStudyComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        protected ICourseService CourseService { get; set; }

        protected void CourseClickedHandler(CourseDetailsModel courseDetails)
        {
            if (currentUserRole == Role.Teacher && courseDetails.TeacherId == currentUserId)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/courses/{courseDetails.Id}/home");
            }
            else if (currentUserRole == Role.Student)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/my-courses/{courseDetails.Id}/home");
            }
        }
    }
}
