using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Student : User
    {
        public Student()
        {
            CourseEnrollments = new HashSet<StudentCourse>();
            StudentStudyGroups = new HashSet<StudentStudyGroup>();
            Assignments = new HashSet<StudentAssignment>();
        }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }

        public ICollection<StudentStudyGroup> StudentStudyGroups { get; set; }

        public ICollection<StudentAssignment> Assignments { get; set; }
    }
}
