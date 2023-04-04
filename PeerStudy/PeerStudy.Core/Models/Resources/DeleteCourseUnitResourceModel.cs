using System;

namespace PeerStudy.Core.Models.Resources
{
    public class DeleteCourseUnitResourceModel
    {
        public Guid CourseUnitId { get; set; }

        public Guid ResourceId { get; set; }
    }
}
