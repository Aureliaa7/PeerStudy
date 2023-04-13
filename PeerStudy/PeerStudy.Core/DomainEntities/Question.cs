using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
            QuestionTags = new HashSet<QuestionTag>();
        }

        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public Student Author { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
