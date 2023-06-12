namespace PeerStudy.Core.Models.Emails
{
    public class NewCourseResourceEmailModel : EmailBaseModel
    {
        public string TeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string ResourceName { get; set; }
    }
}
