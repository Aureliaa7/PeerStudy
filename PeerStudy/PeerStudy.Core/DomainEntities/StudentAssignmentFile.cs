using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentAssignmentFile : Resource
    {
        public Guid Id { get; set; }

        public Guid StudentAssignmentId { get; set; }

        public StudentAssignment StudentAssignment { get; set; }
    }
}
