using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentCourse
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid CourseId { get; set; }

        public Student Student { get; set; }

        public Course Course { get; set; }
    }
}
