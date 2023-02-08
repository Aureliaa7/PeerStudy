using System;

namespace PeerStudy.Core.Models.CourseEnrollments
{
    public class CourseEnrollmentRequestDetailsModel
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid CourseId { get; set; }

        public string StudentName { get; set; }

        public string CourseTitle { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
