using System;

namespace PeerStudy.Core.Models.ProgressModels
{
    public class StudentProgressModel
    {
        public string Name { get; set; }

        public int NoPoints { get; set; }

        public int Rank { get; set; }

        public string ProfilePhotoName { get; set; }

        public Guid StudentId { get; set; }
    }
}
