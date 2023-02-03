using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class ActiveCourses
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        private string addCourseBtnStyle = "position: fixed; right: 30px; margin-bottom: 15px";
        private bool showErrorMessage;
        private bool displayCreateCourseDialog = false;
        private string noCoursesMessage;
        private bool showArchiveCourseResult;
        private string archiveCourseMessage;

        public ActiveCourses(): base(CourseStatus.Active)
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeCoursesListAsync();
            if (isTeacher)
            {
                noCoursesMessage = "There are no active courses. You can create one by clicking on the plus button.";
            }
            else if (isStudent)
            {
                noCoursesMessage = "There are no active courses...";
            }
        }

        public void ShowAddCourseDialog()
        {
            displayCreateCourseDialog = true;
        }

        public async Task SaveCourse(CreateCourseModel courseData)
        {
            bool isValidData = ModelValidator.IsModelValid<CreateCourseModel>(courseData);
            if (isValidData)
            {
                try
                {
                    displayCreateCourseDialog = false;
                    courseData.TeacherId = currentUserId;
                    var addedCourse = await CourseService.AddAsync(courseData);
                    courses.Add(addedCourse);
                    courseData = new CreateCourseModel();
                }
                catch (Exception)
                {
                    showErrorMessage = true;
                }
            }
        }

        public void HideAddCourseDialog()
        {
            displayCreateCourseDialog = false;
        }

        private async Task EditCourseHandler(Guid courseId)
        {

        }

        private async Task ArchiveCourseHandler(Guid courseId)
        {
            if (await CourseService.ArchiveCourseAsync(currentUserId, courseId))
            {
                courses = courses.Where(x => x.Id != courseId).ToList();
                showArchiveCourseResult = true;
                archiveCourseMessage = "The course was successfully archived...";
            }
            else
            {
                showArchiveCourseResult = true;
                archiveCourseMessage = "The course could not be archived...";
            }
        }
    }
}
