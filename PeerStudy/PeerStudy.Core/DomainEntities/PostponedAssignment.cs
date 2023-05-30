using System;

namespace PeerStudy.Core.DomainEntities
{
    public class PostponedAssignment
    {
        public Guid Id { get; set; }

        public Guid StudyGroupId { get; set; }

        public Guid AssignmentId { get; set; }

        public Guid StudentId { get; set; }

        public int NoTotalPaidPoints { get; set; }

        public DateTime CreatedAt { get; set; }

        public StudyGroup StudyGroup { get; set; }

        public Assignment Assignment { get; set; }

        public Student Student { get; set; }
    }
}
