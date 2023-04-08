using PeerStudy.Core.Enums;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.StudyGroups
{
    public class StudentStudyGroupDetailsModel : StudyGroupDetailsModel
    {
        public Dictionary<WorkItemStatus, int> MyWorkItemsStatus { get; set; }
    }
}
