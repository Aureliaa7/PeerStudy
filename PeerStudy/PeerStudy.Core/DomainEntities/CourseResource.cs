using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class CourseResource : Resource
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public ResourceType Type { get; set; }

        public Course Course { get; set; }

        public Teacher Owner { get; set; }
    }
}
