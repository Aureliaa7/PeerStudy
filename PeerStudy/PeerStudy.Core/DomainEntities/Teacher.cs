using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Teacher : User
    {
        public Teacher()
        {
            TeacherCourses = new List<Course>();
            CourseResources = new List<CourseResource>();
        }

        public ICollection<Course> TeacherCourses { get; set; }

        public ICollection<CourseResource> CourseResources { get; set; }
    }
}
