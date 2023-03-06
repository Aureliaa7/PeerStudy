using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class GradeAssignmentModel
    {
        public Guid StudentId { get; set; } 

        public string StudentName { get; set; }

        public int Points { get; set; }
    }
}
