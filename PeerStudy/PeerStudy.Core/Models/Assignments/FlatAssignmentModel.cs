using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class FlatAssignmentModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string CourseTitle { get; set; }

        public DateTime? Deadline { get; set; }

        public Guid CourseId { get; set; }

        public Guid StudyGroupId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string CourseUnitTitle { get; set; }
    }
}
