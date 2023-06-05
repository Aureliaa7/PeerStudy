using PeerStudy.Core.Models.Courses;
using PeerStudy.Core.Models.StudentAssets;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class ExtendedStudentCourseProgressModel : StudentCourseProgressModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public List<StudentBadgeDetailsModel> QAndABadges { get; set; }

        public StudentBadgeDetailsModel? CourseBadge { get; set; }
    }
}
