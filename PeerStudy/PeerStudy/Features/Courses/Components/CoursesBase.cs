using Fluxor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Features.Courses.Store;

namespace PeerStudy.Features.Courses.Components
{
    public abstract class CoursesBase : PeerStudyComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IDispatcher Dispatcher { get; set; }

        [Inject]
        protected IState<CoursesState> CoursesState { get; set; }

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
