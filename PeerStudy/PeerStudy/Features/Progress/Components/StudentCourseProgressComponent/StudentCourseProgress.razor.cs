using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Progress.Components.StudentCourseProgressComponent
{
    public partial class StudentCourseProgress : PeerStudyComponentBase
    {
        [Inject]
        private IAchievementService AchievementService { get; set; }


        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        private ExtendedStudentCourseProgressModel studentCourseProgress;

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            studentCourseProgress = await AchievementService.GetProgressByCourseAndStudentAsync(CourseId, StudentId, currentUserId);
        }
    }
}
