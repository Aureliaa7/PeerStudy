using System;

namespace PeerStudy.Core.DomainEntities
{
    public class Resource
    {
        public string FileName { get; set; }

        public string DriveFileId { get; set; }

        public DateTime CreatedAt { get; set; }  

        public Guid OwnerId { get; set; }
    }
}
