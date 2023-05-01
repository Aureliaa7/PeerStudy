using System;

namespace PeerStudy.Core.Models.StudentAssets
{
    public class SaveStudentPointsModel
    {
        public Guid StudentId { get; set; }

        public int NoPoints { get; set; }
    }
}
