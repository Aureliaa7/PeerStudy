namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupAssignmentsStatisticsDataModel
    {
        public int CompletedOnTimeAssignments { get; set; }

        public int MissingAssignments { get; set; }

        public int DoneLateAssignments { get; set; }

        public int ToDoAssignments { get; set; }
    }
}
