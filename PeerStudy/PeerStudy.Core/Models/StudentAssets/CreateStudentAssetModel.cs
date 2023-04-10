using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.Models.StudentAssets
{
    public class CreateStudentAssetModel
    {
        public AssetType AssetType { get; set; }

        public Guid StudentId { get; set; }

        public int NoAssets { get; set; }
    }
}
