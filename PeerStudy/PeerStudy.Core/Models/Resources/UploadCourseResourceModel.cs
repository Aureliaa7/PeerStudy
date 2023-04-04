using System;

namespace PeerStudy.Core.Models.Resources
{
    public class UploadCourseResourcesModel : UploadResourceModel
    {
        public Guid CourseUnitId { get; set; }

        public Guid CourseId { get; set; }
    }
}
