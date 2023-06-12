namespace PeerStudy.Core.Models.Emails
{
    public class CourseEnrollmentRequestStatusEmailModel : EmailBaseModel
    {
        public string CourseTitle { get; set; }

        public string TeacherName { get; set; }
    }
}
