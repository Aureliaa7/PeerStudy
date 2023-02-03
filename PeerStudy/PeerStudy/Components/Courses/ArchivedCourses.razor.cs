using PeerStudy.Core.Enums;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class ArchivedCourses : CoursesBase
    {
        private string noCoursesMessage = "There are no archived courses yet...";

        public ArchivedCourses(): base(CourseStatus.Archived)
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeCoursesListAsync();
        }
    }
}
