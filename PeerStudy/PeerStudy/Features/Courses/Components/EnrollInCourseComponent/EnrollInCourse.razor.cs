using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.EnrollInCourseComponent
{
    public partial class EnrollInCourse : PeerStudyComponentBase
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        [Inject]
        private ICourseEnrollmentService CourseEnrollmentService { get; set; }


        private const string noCoursesMessage = "There are no active courses...";

        private List<CourseDetailsModel> courses = new List<CourseDetailsModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await SetCurrentUserDataAsync();
                courses = await CourseService.GetCoursesToEnroll(currentUserId);
            }
            catch (Exception ex)
            {
                ToastService.ShowError("An error occurred while fetching courses...");
            }
        }

        private async Task EnrollHandler(CourseDetailsModel courseDetails)
        {
            try
            {
                await CourseEnrollmentService.CreateEnrollmentRequestAsync(currentUserId, courseDetails.Id);
                courses = courses.Except(new List<CourseDetailsModel> { courseDetails }).ToList();
                ToastService.ShowSuccess("The enrollment request was successfully created.");
            }
            catch (Exception ex)
            {
                ToastService.ShowError("The enrollment request could not be created...");
            }
        }
    }
}
