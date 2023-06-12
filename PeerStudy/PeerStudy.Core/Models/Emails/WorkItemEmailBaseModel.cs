namespace PeerStudy.Core.Models.Emails
{
    public class WorkItemEmailBaseModel : EmailBaseModel
    {
        public string WorkItemTitle { get; set; }

        public string StudyGroupTitle { get; set; }

        public string ChangedBy { get; set; }

        public string CourseTitle { get; set; }
    }
}
