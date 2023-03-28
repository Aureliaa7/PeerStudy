using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudyGroupFile : Resource
    {
        public Guid Id { get; set; }

        public Guid StudyGroupId { get; set; }

        public StudyGroup StudyGroup { get; set; }

        public Student Owner { get; set; }
    }
}
