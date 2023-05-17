using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Users;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Students.Components.HomePageComponent
{
    public partial class StudentHomePage : PeerStudyComponentBase
    {
        [Inject]
        private IAchievementService AchievementService { get; set; }


        private StudentProfileModel studentProgress;

        private const string noCompletedAssignmentsMessage = "There are no completed assignments yet...";
        private const string noUnlockedCourseUnits = "There are no unlocked course units yet...";
        private const string noProgressMessage = "No progress yet...";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            studentProgress = await AchievementService.GetProgressByStudentIdAsync(currentUserId);
        }

        private bool HasCompletedAssignments()
        {
            return studentProgress.CoursesProgress
                .Any(x => x.CourseUnitsAssignmentsProgress.Any(y => y.StudentAssignments.Any()));
        }
    }
}
