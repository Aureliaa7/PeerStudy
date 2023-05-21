using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Progress.Components.CourseProgressComponent
{
    public partial class CourseProgress : PeerStudyComponentBase, IDisposable
    {
        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        [Inject]
        private IAchievementService AchievementService { get; set; }

        private const int studyGroupsLeaderboardsTabIndex = 0;
        private const int courseLeaderboardTabIndex = 1;
        private const int courseUnitsLeaderboardsTabIndex = 2;

        private List<StudyGroupLeaderboardModel> studyGroupsProgress = new List<StudyGroupLeaderboardModel>();
        private List<StudentProgressModel> studentsCourseProgress = new List<StudentProgressModel>();
        private List<CourseUnitLeaderboardModel> courseUnitsProgress = new List<CourseUnitLeaderboardModel>();

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            studyGroupsProgress = await AchievementService.GetLeaderboardDataForStudyGroupsAsync(CourseId, currentUserId);
            NavigationMenuService.AddCourseNavigationMenuItems(
                currentUserId,
                CourseId,
                CourseTitle,
                currentUserRole);
        }

        private async Task HandleTabChangedEvent(int activeTabIndex)
        {
            if (activeTabIndex == studyGroupsLeaderboardsTabIndex)
            {
                studyGroupsProgress = await AchievementService.GetLeaderboardDataForStudyGroupsAsync(CourseId, currentUserId);
            }
            else if (activeTabIndex == courseLeaderboardTabIndex)
            {
                studentsCourseProgress = await AchievementService.GetLeaderboardDataByCourseAsync(CourseId, currentUserId);
            }
            else if (activeTabIndex == courseUnitsLeaderboardsTabIndex)
            {
                courseUnitsProgress = await AchievementService.GetCourseUnitsLeaderboardDataAsync(CourseId, currentUserId);
            }
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }

    }
}
