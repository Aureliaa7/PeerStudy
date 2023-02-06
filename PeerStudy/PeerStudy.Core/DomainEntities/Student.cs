using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Student : User
    {
        public Student()
        {
            Courses = new HashSet<Course>();
        }

        public ICollection<Course> Courses { get; set; }
    }
}
