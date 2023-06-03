using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudyGroupAssignmentsModel
    {
        public Guid AssignmentId { get; set; }

        public string AssignmentTitle { get; set; }

        public int NoMaxPoints { get; set; }

        public List<StudentAssignmentStatusModel> StudentAssignmentsStatus { get; set;}
    }
}
