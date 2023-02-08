using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class CourseEnrollmentRequest
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid CourseId { get; set; }

        public CourseEnrollmentRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public Student Student { get; set; }

        public Course Course { get; set; }
    }
}
