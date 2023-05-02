using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.ArchivedCoursesComponent
{
    public partial class ArchivedCourses : CoursesBase
    {
        private const string noCoursesMessage = "There are no archived courses yet...";

        private List<CourseDetailsModel> courses = new List<CourseDetailsModel>();
       
        protected override async Task InitializeAsync()
        {
            ResetNavigationBar();
            await SetCurrentUserDataAsync();
            if (currentUserRole == Role.Student)
            {
                courses = await CourseService.GetCoursesForStudentAsync(currentUserId, CourseStatus.Archived);
            }
            else if (currentUserRole == Role.Teacher)
            {
                courses = await CourseService.GetAsync(currentUserId, CourseStatus.Archived);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
