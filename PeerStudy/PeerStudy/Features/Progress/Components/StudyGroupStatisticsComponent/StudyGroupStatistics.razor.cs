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

        //ToDo: add more colors
        List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };


        private DropDownItem selectedStudyGroup;
        public List<DropDownItem> studyGroupsDropdownItems;
        private StudyGroupStatisticsDataModel studyGroupStatisticsDataModel;

        private BarChart<double> assignmentsProgressBarChart;
        private Chart<double> unlockedCourseUnitsBarChart;
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
            selectedStudyGroup = studyGroupsDropdownItems.First();
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
            await HandleRedrawAssignmentsProgressChart();
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
                .ToList()
                .ConvertAll(x => (double) x);

           var dataset = new BarChartDataset<double>
           {
               Label = "No. unlocked course units",
               Data = data,
               BackgroundColor = backgroundColors,
               BorderColor = borderColors,
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
            var a = studyGroupStatisticsDataModel.AssignmentsProgress.First()
            .StudentAssignmentStatus.First();

            var namesIdsPairs = studyGroupStatisticsDataModel.AssignmentsProgress.First()
            .StudentAssignmentStatus.First()
            .StudentAssignmentsStatus
            .Select(x => new { x.Id, x.FullName })
            .ToDictionary(x => x.Id, x => x.FullName);

            return namesIdsPairs;
        }

        private async Task HandleRedrawAssignmentsProgressChart()
        {
            await assignmentsProgressBarChart.Clear();

            var labels = GetAssignmentsProgressLabels();
            var studentsNameIdPairs = GetStudentsIdsNamesPairs();
            List<BarChartDataset<double>> datasets = new List<BarChartDataset<double>>();

            foreach (var pair in studentsNameIdPairs)
            {
                var dataset = GetStudentAssignmentDataSet(pair);
                datasets.Add(dataset);
            }

            await assignmentsProgressBarChart.AddLabelsDatasetsAndUpdate(labels, datasets.ToArray());
        }

        private BarChartDataset<double> GetStudentAssignmentDataSet(KeyValuePair<Guid, string> studentIdName)
        {
            var data = studyGroupStatisticsDataModel.AssignmentsProgress
                .SelectMany(x => x.StudentAssignmentStatus
                    .SelectMany(y => y.StudentAssignmentsStatus
                        .Where(x => x.Id == studentIdName.Key)
                            .Select(z => z.NoEarnedPoints)
                    )
                 )
                .ToList()
                .ConvertAll(x => (double) x);

            return new BarChartDataset<double>
            {
                Label = studentIdName.Value,
                Data = data,
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
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
                BackgroundColor = backgroundColors
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
