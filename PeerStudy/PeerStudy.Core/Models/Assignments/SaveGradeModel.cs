using System;

namespace PeerStudy.Core.Models.Assignments
{
    public class SaveGradeModel
    {
        public Guid AssignmentId { get; set; }

        public Guid StudentId { get; set; }

        public int Points { get; set; }
    }
}
