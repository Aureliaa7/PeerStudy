using System;

namespace PeerStudy.Core.Models.Courses
{
    public class CreateCourseModel
    {
        public string Title { get; set; }

        public Guid TeacherId { get; set; }

        public int NumberOfStudents { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
