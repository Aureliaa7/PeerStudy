using PeerStudy.Core.Models.Users;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.StudyGroups
{
    public class StudyGroupDetailsModel
    {
        public string Title { get; set; }

        public List<EnrolledStudentModel> Students { get; set; }

        public string CourseTitle { get; set; }
    }
}
