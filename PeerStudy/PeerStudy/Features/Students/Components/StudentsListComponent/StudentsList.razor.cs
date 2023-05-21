using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Students.Components.StudentsListComponent
{
    public partial class StudentsList : PeerStudyComponentBase, IDisposable
    {
        [Inject]
        private ICourseService CourseService { get; set; }


        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        private const string noEnrolledStudentsMessage = "There are no students enrolled yet...";
        private List<EnrolledStudentModel> students = new List<EnrolledStudentModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await SetCurrentUserDataAsync();
                NavigationMenuService.AddCourseNavigationMenuItems(
                currentUserId,
                CourseId,
                CourseTitle,
                currentUserRole);
                students = await CourseService.GetStudentsAsync(CourseId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "The students list could not be loaded...");
            }
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
