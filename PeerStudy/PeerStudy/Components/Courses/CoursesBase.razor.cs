using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public class CoursesBase : ComponentBase
    {
        [Inject]
        IAuthService AuthService { get; set; }

        [Inject]
        ICourseService CourseService { get; set; }

        protected List<CourseDetailsModel> courses = new List<CourseDetailsModel>();

        protected Guid currentUserId;
        protected bool isTeacher;
        protected bool isStudent;
        protected bool isLoading;

        private CourseStatus courseStatus;

        public CoursesBase(CourseStatus courseStatus)
        {
            this.courseStatus = courseStatus;
        }

        protected async Task InitializeCoursesListAsync()
        {
            isLoading = true;
            await InitializeAsync();
            courses = await GetCoursesAsync();
            isLoading = false;
        }

        private async Task InitializeAsync()
        {
            currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
            string userRole = await AuthService.GetCurrentUserRole();
            isTeacher = userRole == Role.Teacher.ToString();
            isStudent = userRole == Role.Student.ToString();
        }

        private Task<List<CourseDetailsModel>> GetCoursesAsync()
        {
            if (isTeacher)
            {
                return CourseService.GetAsync(currentUserId, courseStatus);
            }
            return Task.FromResult(new List<CourseDetailsModel>());
        }
    }
}
