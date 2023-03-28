using PeerStudy.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.WorkItems
{
    public class CreateUpdateWorkItemModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public WorkItemStatus Status { get; set; }

        public Guid? AssignedTo { get; set; }

        public Guid StudyGroupId { get; set; }
    }
}
