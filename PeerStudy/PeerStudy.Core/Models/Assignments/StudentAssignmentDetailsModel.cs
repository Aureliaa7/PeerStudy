using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class StudentAssignmentDetailsModel
    {
        public DateTime CompletedAt { get; set; }

        public Guid AssignmentId { get; set; }

        public string AssignmentTitle { get; set; }

        public int NoEarnedPoints { get; set; }
    }
}
