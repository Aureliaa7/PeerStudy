using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class AssignmentDetailsModel
    {
        public Guid Id { get; set; }

        public Guid StudentGroupId { get; set; }

        public string Title { get; set; }

        public DateTime? Deadline { get; set; }

        public string Description { get; set; }

        public int? Points { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
