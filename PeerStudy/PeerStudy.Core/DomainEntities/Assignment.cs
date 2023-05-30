using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Assignment
    {
        public Assignment()
        {
            StudentAssignments = new HashSet<StudentAssignment>();
            PostponedAssignments = new HashSet<PostponedAssignment>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int Points { get; set; }

        public Guid CourseUnitId { get; set; }

        public CourseUnit CourseUnit { get; set; }

        public Guid StudyGroupId { get; set; }

        public StudyGroup StudyGroup { get; set; }

        public ICollection<StudentAssignment> StudentAssignments { get; set; }

        public ICollection<PostponedAssignment> PostponedAssignments { get; set; }
    }
}
