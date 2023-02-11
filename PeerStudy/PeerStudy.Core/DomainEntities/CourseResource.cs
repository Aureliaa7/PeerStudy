using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class CourseResource
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string DriveFileId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CourseId { get; set; }

        public ResourceType Type { get; set; }

        public Course Course { get; set; }
    }
}
