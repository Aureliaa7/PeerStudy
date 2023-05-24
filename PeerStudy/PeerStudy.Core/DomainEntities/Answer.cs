using System;
using System.Collections.Generic;

namespace PeerStudy.Core.DomainEntities
{
    public class Answer
    {
        public Answer()
        {
            Votes = new HashSet<AnswerVote>();       
        }

        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid AuthorId { get; set; }

        public Guid QuestionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public User Author { get; set; }

        public Question Question { get; set; }

        public ICollection<AnswerVote> Votes { get; set; }
    }
}
