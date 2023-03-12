using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadCourseResourcesModel
    {
        public Guid CourseId { get; set; }

        public List<UploadDriveFileModel> Resources { get; set; }
    }
}
