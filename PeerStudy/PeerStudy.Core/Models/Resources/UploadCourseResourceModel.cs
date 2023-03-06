using System;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadCourseResourceModel : UploadDriveFileModel
    {
        public Guid CourseId { get; set; }
    }
}
