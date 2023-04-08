using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadAssignmentFilesModel
    {
        public Guid AssignmentId { get; set; }

        public Guid StudyGroupId { get; set; }

        public Guid OwnerId { get; set; }

        public List<UploadFileModel> Files { get; set; }
    }
}
