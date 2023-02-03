using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.Courses
{
    public class CourseModel
    {
        [Required]
        public string Title { get; set; }

        public Guid TeacherId { get; set; }

        [Required]
        public int NumberOfStudents { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
