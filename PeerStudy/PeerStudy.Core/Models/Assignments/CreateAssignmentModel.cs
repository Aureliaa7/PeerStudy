using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.Assignments
{
    public class CreateAssignmentModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public Guid TeacherId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(1, 100)]
        public int Points { get; set; }

        public Guid StudyGroupId { get; set; }

        public Guid CourseUnitId { get; set; }
    }
}
