using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class CourseUnit
    {
        public CourseUnit()
        {
            Resources = new HashSet<CourseResource>();
            Assignments = new HashSet<Assignment>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public bool IsAvailable { get; set; }

        public int NoPointsToUnlock { get; set; }

        public ICollection<CourseResource> Resources { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
    }
}
