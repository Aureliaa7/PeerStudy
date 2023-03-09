using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;

namespace PeerStudy.Components.Courses
{
    public abstract class CoursesBase: PeerStudyComponentBase<CourseDetailsModel>
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected void CourseClickedHandler(CourseDetailsModel courseDetails)
        {
            if (isTeacher && courseDetails.TeacherId == currentUserId)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/courses/{courseDetails.Id}/resources");
            }
            else if (isStudent)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/my-courses/{courseDetails.Id}/resources");
            }
        }
    }
}
