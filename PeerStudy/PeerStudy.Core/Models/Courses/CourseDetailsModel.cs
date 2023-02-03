using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.Models.Courses
{
    public class CourseDetailsModel
    {
        public Guid Id { get; set; }    
        public string Title { get; set; }

        public string TeacherName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int NoMaxStudents { get; set; }  

        public int NoEnrolledStudents { get; set; }

        public CourseStatus Status { get; set; }
    }
}
