using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public abstract class CoursesBase : ComponentBase
    {
        [Inject]
        IAuthService AuthService { get; set; }

        protected List<CourseDetailsModel> courses = new List<CourseDetailsModel>();

        protected Guid currentUserId;
        protected bool isTeacher;
        protected bool isStudent;
        protected bool isLoading;

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

        protected abstract Task<List<CourseDetailsModel>> GetCoursesAsync();
    }
}
