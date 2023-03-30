using Blazored.Toast.Services;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Features.Courses.Store;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.ActiveCoursesComponent
{
    public partial class ActiveCourses : CoursesBase
    {
        private const string addCourseBtnStyle = "position: fixed; right: 30px; margin-bottom: 15px";
        private const string courseCreationMessage = "Course creation is in progress...";

        private bool displayCourseDialog = false;
        private string noCoursesMessage;
        private bool isEditCourseModeEnabled;
        private CourseModel CourseModel = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            ResetNavigationBar();
            await SetCurrentUserDataAsync();
            Dispatcher.Dispatch(new FetchActiveCoursesAction(currentUserId, currentUserRole));

            if (currentUserRole == Role.Teacher)
            {
                noCoursesMessage = "There are no active courses. You can create one by clicking on the plus button.";
            }
            else if (currentUserRole == Role.Student)
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
                ToastService.ShowToast(ToastLevel.Info, courseCreationMessage, false);
                displayCourseDialog = false;

                // fix for MatDatePicker
                CourseModel.StartDate = CourseModel.StartDate.AddDays(1);
                CourseModel.EndDate = CourseModel.EndDate.AddDays(1);

                CourseModel.TeacherId = currentUserId;
                Dispatcher.Dispatch(new AddCourseAction(CourseModel));
                CourseModel = new CourseModel();
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

        private void EditCourseHandler(CourseDetailsModel course)
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

        private void EditCourse()
        {
            displayCourseDialog = false;
            isEditCourseModeEnabled = false;
            Dispatcher.Dispatch(new UpdateCourseAction((UpdateCourseModel)CourseModel));

            CourseModel = new CourseModel();
        }

        private void ArchiveCourseHandler(Guid courseId)
        {
            Dispatcher.Dispatch(new ArchiveCourseAction(courseId, currentUserId));
        }
    }
}
