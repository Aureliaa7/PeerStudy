using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.StudentAssets;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Users
{
    public class StudentProfileModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public int NoTotalPoints { get; set; }

        public List<StudentCourseProgressModel> CoursesProgress { get; set; }

        public List<StudentBadgeDetailsModel> QAndABadges { get; set; }

        public List<StudentBadgeDetailsModel> CoursesBadges { get; set; }

        public List<CourseRankingModel> CourseRankings { get; set; }
    }
}
