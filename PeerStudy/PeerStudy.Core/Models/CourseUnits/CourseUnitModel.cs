using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.CourseUnits
{
    public class CourseUnitModel
    {
        public Guid CourseId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        public int NoPointsToUnlock { get; set; }
    }
}
