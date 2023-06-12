using Blazored.Toast.Services;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.ProgressModels;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PeerStudy.ClientConstants;
using StringExtensions = PeerStudy.Core.Extensions.StringExtensions;

namespace PeerStudy.Features.Progress.Components.StudyGroupStatisticsComponent
{
    public partial class StudyGroupStatistics : PeerStudyComponentBase, IDisposable
    {
        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        // also use the id of the current user to get the data
        [Inject]
        private IStatisticsService StatisticsService { get; set; }

        [Inject]
        private IStudyGroupService StudyGroupService { get; set; }

        private DropDownItem? selectedStudyGroup;
        public List<DropDownItem> studyGroupsDropdownItems;
        private StudyGroupStatisticsDataModel studyGroupStatisticsDataModel;

        private BarChart<int> assignmentsProgressBarChart;
        private Chart<int> unlockedCourseUnitsBarChart;
        private PieChart<int> assignmentsStatisticsChart;

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            NavigationMenuService.AddCourseNavigationMenuItems(
                currentUserId,
                CourseId,
                CourseTitle,
                currentUserRole);
            await SetStudyGroupsDropdownItems();
            selectedStudyGroup = studyGroupsDropdownItems.Any() ? studyGroupsDropdownItems.First() : new DropDownItem();
            studyGroupStatisticsDataModel = await StatisticsService.GetStatisticsDataByGroupAsync(new Guid(selectedStudyGroup.Key), CourseId);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            StateHasChanged();
            await DrawCharts();
        }

        private async Task SetStudyGroupsDropdownItems()
        {
            studyGroupsDropdownItems = (await StudyGroupService.GetStudyGroupIdNamePairsAsync(CourseId))
                .Select(x => new DropDownItem
                {
                    Key = x.Key.ToString(),
                    Value = x.Value
                })
                .ToList();
        }

        private async Task HandleStudyGroupSelection(DropDownItem selectedStudyGroup)
        {
            isLoading = true;

            try
            {
                this.selectedStudyGroup = selectedStudyGroup;
                studyGroupStatisticsDataModel = await StatisticsService.GetStatisticsDataByGroupAsync(new Guid(selectedStudyGroup.Key), CourseId);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
            }
            isLoading = false;
            StateHasChanged();

            await DrawCharts();
        }

        private async Task DrawCharts()
        {
            await DrawAssignmentsProgressChart();
            await DrawUnlockedCourseUnitsStatisticsChart();
            await DrawAssignmentsStatisticsChart();
        }

        #region Charts logic

        #region common logic
        private static BarChartOptions GetBarChartOptions(string title)
        {
            BarChartOptions options = new()
            {
                IndexAxis = "x",
                Elements = new()
                {
                    Bar = new()
                    {
                        BorderWidth = 2,
                    }
                },
                Responsive = true,
                Plugins = new()
                {
                    Legend = new()
                    {
                        Position = "top"
                    },
                    Title = new()
                    {
                        Display = true,
                        Text = title
                    }
                }
            };

            return options;
        }

        #endregion

        #region unlocked course units
        private async Task DrawUnlockedCourseUnitsStatisticsChart()
        {
            await unlockedCourseUnitsBarChart.Clear();

            var studentNameNoUnlockedCourseUnitsPairs = studyGroupStatisticsDataModel.UnlockedCourseUnits.Select(
                x => new { x.FullName, x.UnlockedCourseUnits })
                .ToDictionary(x => x.FullName, x => x.UnlockedCourseUnits);

            var labels = studentNameNoUnlockedCourseUnitsPairs.Select(x => x.Key)
                .ToList();

            var data = studentNameNoUnlockedCourseUnitsPairs.Select(x => x.Value)
                .ToList();

           var dataset = new BarChartDataset<int>
           {
               Label = "No. unlocked course units",
               Data = data,
               BackgroundColor = ChartColors.BackgroundColors,
               BorderColor = ChartColors.BorderColors,
               BorderWidth = 1
           };

            await unlockedCourseUnitsBarChart.AddLabelsDatasetsAndUpdate(labels, dataset);
        }
  
        #endregion

        #region assignments progress chart
        private List<string> GetAssignmentsProgressLabels()
        {
            var labels = studyGroupStatisticsDataModel.AssignmentsProgress
                .SelectMany(x => x.StudentAssignmentStatus
                .Select(y => y.AssignmentTitle))
                .ToList();

            return labels;
        }

        private Dictionary<Guid, string> GetStudentsIdsNamesPairs()
        {
            var namesIdsPairs = studyGroupStatisticsDataModel.AssignmentsProgress.First()
            .StudentAssignmentStatus.First()
            .StudentAssignmentsStatus
            .Select(x => new { x.Id, x.FullName })
            .ToDictionary(x => x.Id, x => x.FullName);

            return namesIdsPairs;
        }

        private async Task DrawAssignmentsProgressChart()
        {
            await assignmentsProgressBarChart.Clear();

            var labels = GetAssignmentsProgressLabels();
            var studentsNameIdPairs = GetStudentsIdsNamesPairs();
            List<BarChartDataset<int>> datasets = new List<BarChartDataset<int>>();

            foreach (var pair in studentsNameIdPairs)
            {
                var dataset = GetStudentAssignmentDataSet(pair);
                datasets.Add(dataset);
            }

            await assignmentsProgressBarChart.AddLabelsDatasetsAndUpdate(labels, datasets.ToArray());
        }

        private BarChartDataset<int> GetStudentAssignmentDataSet(KeyValuePair<Guid, string> studentIdName)
        {
            var data = studyGroupStatisticsDataModel.AssignmentsProgress
                .SelectMany(x => x.StudentAssignmentStatus
                    .SelectMany(y => y.StudentAssignmentsStatus
                        .Where(x => x.Id == studentIdName.Key)
                            .Select(z => z.NoEarnedPoints)
                    )
                 )
                .ToList();

            return new BarChartDataset<int>
            {
                Label = studentIdName.Value,
                Data = data,
                BackgroundColor = ChartColors.BackgroundColors,
                BorderColor = ChartColors.BorderColors,
            };
        }
        #endregion

        #region assignments statistics
        private async Task DrawAssignmentsStatisticsChart()
        {
            await assignmentsStatisticsChart.Clear();

            var dataset = new PieChartDataset<int>
            {
                Data = new List<int> 
                { 
                    studyGroupStatisticsDataModel.AssignmentsStatistics.CompletedOnTimeAssignments,
                    studyGroupStatisticsDataModel.AssignmentsStatistics.DoneLateAssignments,
                    studyGroupStatisticsDataModel.AssignmentsStatistics.MissingAssignments,
                    studyGroupStatisticsDataModel.AssignmentsStatistics.ToDoAssignments,
                },
                BackgroundColor = ChartColors.BackgroundColors
            };

            var labels = new List<string>
            {
                StringExtensions.SplitByUpperLetterAndJoinBySpace(nameof(studyGroupStatisticsDataModel.AssignmentsStatistics.CompletedOnTimeAssignments)),
                StringExtensions.SplitByUpperLetterAndJoinBySpace(nameof(studyGroupStatisticsDataModel.AssignmentsStatistics.DoneLateAssignments)),
                StringExtensions.SplitByUpperLetterAndJoinBySpace(nameof(studyGroupStatisticsDataModel.AssignmentsStatistics.MissingAssignments)),
                StringExtensions.SplitByUpperLetterAndJoinBySpace(nameof(studyGroupStatisticsDataModel.AssignmentsStatistics.ToDoAssignments))
            };
            
            await assignmentsStatisticsChart.AddLabelsDatasetsAndUpdate(labels, dataset);
        }
        #endregion

        #endregion

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
