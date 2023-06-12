namespace PeerStudy.Core.Models.Emails
{
    public class GradedAssignmentEmailModel : AssignmentBaseModel
    {
        public int EarnedPoints { get; set; }

        public int NoMaxPoints { get; set; }
    }
}
