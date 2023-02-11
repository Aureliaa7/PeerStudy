using System;

namespace PeerStudy.Core.Models.Resources
{
    public class CourseResourceDetailsModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }
    }
}
