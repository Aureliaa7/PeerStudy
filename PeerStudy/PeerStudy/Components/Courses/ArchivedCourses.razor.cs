using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class ArchivedCourses : PeerStudyComponentBase<CourseDetailsModel>
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        private const string noCoursesMessage = "There are no archived courses yet...";

        protected override Task<List<CourseDetailsModel>> GetDataAsync()
        {
            if (isTeacher)
            {
                return CourseService.GetAsync(currentUserId, CourseStatus.Archived);
            }
            else if (isStudent)
            {
                return CourseService.GetCoursesForStudentAsync(currentUserId, CourseStatus.Archived);
            }
            return Task.FromResult(new List<CourseDetailsModel>());
        }

        protected override async Task OnInitializedAsync()
        {
            ResetNavigationBar();
            await InitializeDataAsync();
        }
    }
}
