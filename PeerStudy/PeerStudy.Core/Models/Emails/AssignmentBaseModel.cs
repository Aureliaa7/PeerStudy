namespace PeerStudy.Core.Models.Emails
{
    public class AssignmentBaseModel : EmailBaseModel
    {
        public string TeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseUnitTitle { get; set; }

        public string AssignmentTitle { get; set; }
    }
}
