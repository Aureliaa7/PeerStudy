using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.Assignments
{
    public class CreateAssignmentModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid CourseId { get; set; }

        public Guid TeacherId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(1, 100)]
        public int Points { get; set; }
    }
}
