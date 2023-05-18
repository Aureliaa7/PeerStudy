using System;

namespace PeerStudy.Core.Models.StudentAssets
{
    public class CourseRankingModel
    {
        public string CourseTitle { get; set; }

        public int Rank { get; set; }

        public string ProfilePhotoName { get; set; }

        public Guid StudentId { get; set; }

        public string StudentName { get; set; }

        public int EarnedPoints { get; set; }
    }
}
