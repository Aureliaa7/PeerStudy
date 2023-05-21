using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<StudyGroupLeaderboardModel> studyGroupsProgress;
        private List<StudentProgressModel> studentsCourseProgress;
        private List<CourseUnitLeaderboardModel> courseUnitsProgress;
        private int activeTabIndex = 0;


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
            isLoading = true;
            this.activeTabIndex = activeTabIndex;
            try
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
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
            }
            isLoading = false;
        }

        private bool HasData()
        {
            if (activeTabIndex == studyGroupsLeaderboardsTabIndex)
            {
                return !isLoading && studyGroupsProgress != null && studyGroupsProgress.Any();
            }
            else if (activeTabIndex == courseLeaderboardTabIndex)
            {
                return !isLoading && studentsCourseProgress != null && studentsCourseProgress.Any();
            }
            else if (activeTabIndex == courseUnitsLeaderboardsTabIndex)
            {
                return !isLoading && courseUnitsProgress != null && courseUnitsProgress.Any();
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
