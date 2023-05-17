using System;

namespace PeerStudy.Core.DomainEntities
{
    public class UnlockedCourseUnit
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid CourseUnitId { get; set; }

        public DateTime UnlockedAt { get; set; }

        public Student Student { get; set; }

        public CourseUnit CourseUnit { get; set; }
    }
}
