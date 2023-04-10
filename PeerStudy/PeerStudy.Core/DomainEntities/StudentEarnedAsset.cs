using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class StudentAsset
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public AssetType Asset { get; set; }

        public int NumberOfAssets { get; set; }

        public Student Student { get; set; }
    }
}
