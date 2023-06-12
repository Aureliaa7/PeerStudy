namespace PeerStudy.Core.Models.Emails
{
    public class CourseEnrollmentRequestEmailModel : EmailBaseModel
    {
        public string StudentName { get; set; }

        public string CourseTitle { get; set; }
    }
}
