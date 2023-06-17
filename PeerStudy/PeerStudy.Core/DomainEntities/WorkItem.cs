using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class WorkItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public WorkItemStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? AssignedToId { get; set; }

        public Guid StudyGroupId { get; set; }

        public Guid ChangedById { get; set; }

        public StudyGroup StudyGroup { get; set; }

        public Student? AssignedTo { get; set; }

        public Student ChangedBy { get; set; }
    }
}
