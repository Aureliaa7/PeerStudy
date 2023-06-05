using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.Models.StudentAssets
{
    public class StudentBadgeDetailsModel
    {
        public StudentBadgeType Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Base64Content { get; set; }

        public int Points { get; set; }

        public DateTime EarnedAt { get; set; }

        public string? CourseTitle { get; set; }
    }
}
