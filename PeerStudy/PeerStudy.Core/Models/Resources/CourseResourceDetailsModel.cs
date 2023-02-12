using PeerStudy.Core.Models.GoogleDriveModels;
using System;

namespace PeerStudy.Core.Models.Resources
{
    public class CourseResourceDetailsModel : FileDetailsModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }

        public Guid TeacherId { get; set; }

        public string TeacherName { get; set; }
    }
}
