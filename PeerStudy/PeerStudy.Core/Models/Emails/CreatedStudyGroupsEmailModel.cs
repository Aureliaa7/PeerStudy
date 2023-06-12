using System.Collections.Generic;

namespace PeerStudy.Core.Models.Emails
{
    public class CreatedStudyGroupsEmailModel : EmailBaseModel
    {
        public string TeacherName { get; set; }

        public string CourseTitle { get; set; }

        public List<string> StudyGroupMembers { get; set; }
    }
}
