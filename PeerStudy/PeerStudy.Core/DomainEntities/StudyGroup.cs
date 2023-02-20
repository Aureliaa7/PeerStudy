using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class StudyGroup
    {
        public StudyGroup()
        {
            StudentStudyGroups = new HashSet<StudentStudyGroup>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public ICollection<StudentStudyGroup> StudentStudyGroups { get; set; }
    }
}
