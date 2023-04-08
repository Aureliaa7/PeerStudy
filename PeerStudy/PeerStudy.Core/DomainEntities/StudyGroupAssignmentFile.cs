using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudyGroupAssignmentFile : Resource
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }

        public Assignment Assignment { get; set; }
    }
}
