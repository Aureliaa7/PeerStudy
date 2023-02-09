using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class EnrollInCourse : PeerStudyComponentBase<CourseDetailsModel>
    {
        [Inject]
        private ICourseService CourseService { get; set; }

        [Inject]
        private ICourseEnrollmentService CourseEnrollmentService { get; set; }

        private const string noCoursesMessage = "There are no active courses...";
        private bool displayCreateEnrollmentMessage;
        private string createEnrollmentMessage;

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override Task<List<CourseDetailsModel>> GetDataAsync()
        {
            return CourseService.GetCoursesToEnroll(currentUserId);
        }

        private async Task EnrollHandler(CourseDetailsModel courseDetails)
        {
            try
            {
                await CourseEnrollmentService.CreateEnrollmentRequestAsync(currentUserId, courseDetails.Id);
                data = data.Except(new List<CourseDetailsModel> { courseDetails }).ToList();
                createEnrollmentMessage = "The enrollment request was successfully created";
            }
            catch (Exception ex)
            {
                createEnrollmentMessage = "The enrollment request could not be created...";
            }

            displayCreateEnrollmentMessage = true;
        }
    }
}
