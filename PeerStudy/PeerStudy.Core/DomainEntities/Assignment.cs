using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Assignment
    {
        public Assignment()
        {
            StudentAssignments = new HashSet<StudentAssignment>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? Deadline { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
