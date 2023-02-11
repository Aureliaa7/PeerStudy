using System;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadCourseResourceModel : UploadFileModel
    {
        public Guid CourseId { get; set; }
    }
}
