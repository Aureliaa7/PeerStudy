using System;

namespace PeerStudy.Core.Models.CourseUnits
{
    public class UnlockedCourseUnitModel
    {
        public string CourseUnitTitle { get; set; }

        public int NoPaidPoints { get; set; }

        public DateTime UnlockedAt { get; set; }
    }
}
