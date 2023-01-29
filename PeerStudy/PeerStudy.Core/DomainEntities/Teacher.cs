using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Teacher : User
    {
        public Teacher()
        {
            TeacherCourses = new List<Course>();
        }

        public ICollection<Course> TeacherCourses { get; set; }
    }
}
