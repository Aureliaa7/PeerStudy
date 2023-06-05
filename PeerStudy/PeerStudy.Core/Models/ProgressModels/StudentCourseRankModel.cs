using System;

namespace PeerStudy.Core.Models.ProgressModels
{
    internal class StudentCourseRankModel
    {
        public Guid StudentId { get; set; }

        public int Points { get; set; }

        public int Rank { get; set; }
    }
}
