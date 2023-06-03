using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupStatisticsDataModel
    {
        public string StudyGroupName { get; set; }

        public Guid StudyGroupId { get; set; }

        public int NoLockedCourseUnits { get; set; }

        public StudyGroupAssignmentsStatisticsDataModel AssignmentsStatistics { get; set; }

        public List<StudyGroupCourseUnitStatusModel> AssignmentsProgress { get; set; }

        public List<StudentCourseUnitStatisticsDataModel> UnlockedCourseUnits { get; set; }
    }
}
