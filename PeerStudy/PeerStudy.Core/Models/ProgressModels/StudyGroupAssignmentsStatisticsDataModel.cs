using System;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupAssignmentsStatisticsDataModel
    {
        public int NoTotalAssignments { get; set; }

        public int NoCompletedOnTimeAssignments { get; set; }

        public int NoMissingAssignments { get; set; }

        public int NoDoneLateAssignments { get; set; }

        public static implicit operator StudyGroupAssignmentsStatisticsDataModel(StudyGroupStatisticsDataModel v)
        {
            throw new NotImplementedException();
        }
    }
}
