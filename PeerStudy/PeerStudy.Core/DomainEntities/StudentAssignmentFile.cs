using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentAssignmentFile
    {
        public Guid Id { get; set; }

        public Guid StudentAssignmentId { get; set; }

        public string DriveFileId { get; set; }

        public string FileName { get; set; }

        public StudentAssignment StudentAssignment { get; set; }
    }
}
