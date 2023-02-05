using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class ArchivedCourses : CoursesBase
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        private string noCoursesMessage = "There are no archived courses yet...";

        protected override Task<List<CourseDetailsModel>> GetCoursesAsync()
        {
            if (isTeacher)
            {
                return CourseService.GetAsync(currentUserId, CourseStatus.Archived);
            }
            else if (isStudent)
            {
                // get all archived courses for student
                //TODO: should use pagination
            }
            return Task.FromResult(new List<CourseDetailsModel>());
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeCoursesListAsync();
        }
    }
}
