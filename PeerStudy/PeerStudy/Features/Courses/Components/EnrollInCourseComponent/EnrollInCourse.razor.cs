using Blazored.Toast.Services;
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
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GetCoursesErrorMessage);
            }
        }

        private async Task EnrollHandler(CourseDetailsModel courseDetails)
        {
            try
            {
                courses = courses.Except(new List<CourseDetailsModel> { courseDetails }).ToList();
                await CourseEnrollmentService.CreateEnrollmentRequestAsync(currentUserId, courseDetails.Id);
                ToastService.ShowToast(ToastLevel.Success, UIMessages.CreateCourseEnrollmentRequestSuccessMessage);
            }
            catch (Exception ex)
            {
                courses.Add(courseDetails);
                ToastService.ShowToast(ToastLevel.Error, UIMessages.CreateCourseEnrollmentRequestErrorMessage);
            }
        }
    }
}
