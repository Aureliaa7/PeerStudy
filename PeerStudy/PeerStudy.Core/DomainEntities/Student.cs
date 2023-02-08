using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Student : User
    {
        public Student()
        {
            CourseEnrollments = new HashSet<StudentCourse>();
        }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }
    }
}
