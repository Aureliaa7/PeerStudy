using System;

namespace PeerStudy.Core.Models.WorkItems
{
    public class WorkItemDetailsModel : CreateUpdateWorkItemModel
    {
        public Guid Id { get; set; }

        public string AssignedToFullName { get; set; }

        public string StudyGroupName { get; set; }
    }
}
