using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class ActiveCourses : PeerStudyComponentBase<CourseDetailsModel>
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private const string addCourseBtnStyle = "position: fixed; right: 30px; margin-bottom: 15px";
        private const string courseCreationMessage = "Course creation is in progress...";
        private const string courseCreationMessageStyle = "position: absolute; bottom: 10px; width: 50%; left: 0; right: 0; margin: auto; text-align: center;";

        private bool showErrorMessage;
        private bool displayCourseDialog = false;
        private string noCoursesMessage;
        private bool showArchiveCourseResult;
        private string archiveCourseMessage;
        private bool isEditCourseModeEnabled;
        private CourseModel CourseModel = new();
        private bool displayCourseCreationMessage;

        protected override Task<List<CourseDetailsModel>> GetDataAsync()
        {
            if (isTeacher)
            {
                return CourseService.GetAsync(currentUserId, CourseStatus.Active);
            } 
            else if (isStudent)
            {
                return CourseService.GetCoursesForStudentAsync(currentUserId, CourseStatus.Active);
            }
            return Task.FromResult(new List<CourseDetailsModel>());
        }

        protected override async Task OnInitializedAsync()
        {
            ResetNavigationBar();
            await InitializeDataAsync();

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
            displayCourseDialog = true;
        }

        public async Task SaveCourse()
        {
            bool isValidData = ModelValidator.IsModelValid<CourseModel>(CourseModel);
            if (isValidData)
            {
                displayCourseCreationMessage = true;
                try
                {
                    displayCourseDialog = false;
                    CourseModel.TeacherId = currentUserId;
                    var addedCourse = await CourseService.AddAsync(CourseModel);
                    data.Add(addedCourse);
                    CourseModel = new CourseModel();
                }
                catch (Exception)
                {
                    showErrorMessage = true;
                }
                displayCourseCreationMessage = false;
            }
        }

        private void CloseCourseDialog()
        {
            displayCourseDialog = false;
            CourseModel = new();
        }

        private void CancelEditCourse()
        {
            isEditCourseModeEnabled = false;
            CloseCourseDialog();
        }

        private async Task EditCourseHandler(CourseDetailsModel course)
        {
            displayCourseDialog = true;
            isEditCourseModeEnabled = true;
            CourseModel = new UpdateCourseModel
            {
                Title = course.Title,
                EndDate = course.EndDate,
                StartDate = course.StartDate,
                NumberOfStudents = course.NoMaxStudents,
                TeacherId = currentUserId,
                Id = course.Id,
            };
        }

        private async Task EditCourseAsync()
        {
            var course = (UpdateCourseModel)CourseModel;
            var courseToBeUpdated = data.First(x => x.Id == course.Id);

            displayCourseDialog = false;
            var updatedCourse = await CourseService.UpdateAsync((UpdateCourseModel)CourseModel);
            courseToBeUpdated.Title = updatedCourse.Title;
            courseToBeUpdated.NoMaxStudents = updatedCourse.NoMaxStudents;
            CourseModel = new CourseModel();
            StateHasChanged();
        }

        private async Task ArchiveCourseHandler(Guid courseId)
        {
            if (await CourseService.ArchiveCourseAsync(currentUserId, courseId))
            {
                data = data.Where(x => x.Id != courseId).ToList();
                showArchiveCourseResult = true;
                archiveCourseMessage = "The course was successfully archived...";
            }
            else
            {
                showArchiveCourseResult = true;
                archiveCourseMessage = "The course could not be archived...";
            }
        }

        private async Task CourseClickedHandler(CourseDetailsModel courseDetails)
        {
            // redirect the user to specific page
            if (isTeacher && courseDetails.TeacherId == currentUserId)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/courses/{courseDetails.Id}");
            }
            else if (isStudent)
            {
                NavigationManager.NavigateTo($"/{currentUserId}/my-courses/{courseDetails.Id}");
            }
            else
            {
                NavigationManager.NavigateTo("/forbidden");  //TOD0: create forbidden comp.
            }
        }
    }
}
