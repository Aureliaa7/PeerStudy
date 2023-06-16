using Blazored.Toast.Services;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.ActiveCoursesComponent
{
    public partial class ActiveCourses : CoursesBase
    {
        private const string addCourseBtnStyle = "position: fixed; right: 30px; margin-bottom: 15px";
        private const string archivePopupTitle = "Archive Course";
        private const string CreateCourseDialogTitle = "Create course";
        private const string UpdateCourseDialogTitle = "Update course";

        private bool displayCourseDialog = false;
        private string noCoursesMessage;
        private bool isEditCourseModeEnabled;
        private bool courseHasStudyGroups;
        private bool isArchiveConfirmationPopupVisible;
        private CourseModel CourseModel = new();
        private List<CourseDetailsModel> courses = new List<CourseDetailsModel>();
        private Guid? courseIdToBeArchived;
     
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            ResetNavigationBar();
            await SetCurrentUserDataAsync();

            if (currentUserRole == Role.Teacher)
            {
                courses = await CourseService.GetAsync(currentUserId, CourseStatus.Active);
                noCoursesMessage = UIMessages.NoActiveCoursesForTeacherMessage;
            }
            else if (currentUserRole == Role.Student)
            {
                courses = await CourseService.GetCoursesForStudentAsync(currentUserId, CourseStatus.Active);
                noCoursesMessage = UIMessages.NoActiveCoursesForStudentMessage;
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
                ToastService.ShowToast(ToastLevel.Info, UIMessages.CourseCreationMessage, false);
                displayCourseDialog = false;

                // fix for MatDatePicker
                CourseModel.StartDate = CourseModel.StartDate.AddDays(1);
                CourseModel.EndDate = CourseModel.EndDate.AddDays(1);
                CourseModel.TeacherId = currentUserId;

                try
                {
                    var savedCourse = await CourseService.AddAsync(CourseModel);
                    courses.Add(savedCourse);
                    ToastService.ShowToast(ToastLevel.Success, UIMessages.AddCourseSuccessMessage);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, UIMessages.AddCourseErrorMessage);
                }
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
            courseHasStudyGroups = course.HasStudyGroups;

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

        private async Task EditCourse()
        {
            displayCourseDialog = false;
            isEditCourseModeEnabled = false;
            try
            {
                var updatedCourseModel = (UpdateCourseModel)CourseModel;
                await CourseService.UpdateAsync(updatedCourseModel);

                var courseToBeUpdated = courses.First(x => x.Id == updatedCourseModel.Id);
                courseToBeUpdated.Title = updatedCourseModel.Title;
                courseToBeUpdated.NoMaxStudents = updatedCourseModel.NumberOfStudents;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateCourseErrorMessage);
            }

            CourseModel = new CourseModel();
            courseHasStudyGroups = false;
        }

        private void DisplayArchiveConfirmationPopup(Guid courseId)
        {
            courseIdToBeArchived = courseId;
            isArchiveConfirmationPopupVisible = true;
        }

        private void ArchiveCourseHandler()
        {
            isArchiveConfirmationPopupVisible = false;

            try
            {
                CourseService.ArchiveCourseAsync(currentUserId, courseIdToBeArchived.Value);
                var archivedCourse = courses.First(x => x.Id == courseIdToBeArchived.Value);
                courses.Remove(archivedCourse);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.ArchiveCourseErrorMessage);
            }

            courseIdToBeArchived = null;
        }

        private void CancelArchiveCourse()
        {
            isArchiveConfirmationPopupVisible = false;
            courseIdToBeArchived = null;
        }
    }
}
