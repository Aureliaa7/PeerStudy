using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentAssignment
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid AssignmentId { get; set; }

        public Guid StudyGroupId { get; set; }

        public int? Points { get; set; }

        public Student Student { get; set; }

        public Assignment Assignment { get; set; }

        public StudyGroup StudyGroup { get; set; }
    }
}
