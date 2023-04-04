using PeerStudy.Core.Enums;
using System;

//TODO: remove courseId, we can get it from courseUnit
namespace PeerStudy.Core.DomainEntities
{
    public class CourseResource : Resource
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public ResourceType Type { get; set; }

        public Course Course { get; set; }

        public Teacher Owner { get; set; }

        public Guid CourseUnitId { get; set; }

        public CourseUnit CourseUnit { get; set; }
    }
}
