using PeerStudy.Features.Courses.Store;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.ArchivedCoursesComponent
{
    public partial class ArchivedCourses : CoursesBase
    {
        private const string noCoursesMessage = "There are no archived courses yet...";

        protected override async Task InitializeAsync()
        {
            ResetNavigationBar();
            await SetCurrentUserDataAsync();
            Dispatcher.Dispatch(new FetchArchivedCoursesAction(currentUserId, currentUserRole));
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
