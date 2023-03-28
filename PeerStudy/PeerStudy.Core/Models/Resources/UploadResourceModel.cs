using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadResourceModel
    {
        public Guid OwnerId { get; set; }

        public List<UploadDriveFileModel> Resources { get; set; }
    }
}
