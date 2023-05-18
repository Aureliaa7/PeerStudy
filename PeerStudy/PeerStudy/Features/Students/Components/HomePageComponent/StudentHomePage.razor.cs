using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Users;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Students.Components.HomePageComponent
{
    public partial class StudentHomePage : PeerStudyComponentBase
    {
        [Inject]
        private IAchievementService AchievementService { get; set; }

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }


        private StudentProfileModel studentProgress;
        private List<CourseStatus?> courseStatuses = Enum.GetValues(typeof(CourseStatus)).Cast<CourseStatus?>().ToList();
        private CourseStatus? courseStatus = CourseStatus.Active;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            studentProgress = await AchievementService.GetProgressByStudentIdAsync(currentUserId, CourseStatus.Active);
        }

        private bool HasCompletedAssignments()
        {
            return studentProgress.CoursesProgress
                .Any(x => x.CourseUnitsAssignmentsProgress.Any(y => y.StudentAssignments.Any()));
        }

        //TODO: avoid making a DB call every time the user switches the tab and/or changes the course status
        private async Task HandleSelectedStatusChanged(CourseStatus? courseStatus)
        {
            this.courseStatus = courseStatus;
            try
            {
                studentProgress = await AchievementService.GetProgressByStudentIdAsync(currentUserId, courseStatus.Value);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "");
            }
        }
    }
}
