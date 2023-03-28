using PeerStudy.Core.Models.GoogleDriveModels;
using System;

namespace PeerStudy.Core.Models.Resources
{
    public class FileModel : DriveFileDetailsModel
    {
        public string FileName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OwnerId { get; set; }

        public string OwnerName { get; set; }
    }
}
