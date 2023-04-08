using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.StudyGroups
{
    public class StudyGroupDetailsModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<EnrolledStudentModel> Students { get; set; }

        public string CourseTitle { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<WorkItemStatus, int> AllWorkItemsStatus { get; set; }
    }
}
