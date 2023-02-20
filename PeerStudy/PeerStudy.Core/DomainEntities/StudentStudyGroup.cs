using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentStudyGroup
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid StudyGroupId { get; set; }

        public Student Student { get; set; }

        public StudyGroup StudyGroup { get; set; }
    }
}
