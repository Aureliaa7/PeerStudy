using System;

namespace PeerStudy.Core.DomainEntities
{
    public class Answer
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid AuthorId { get; set; }

        public Guid QuestionId { get; set; }

        public int NoUpvotes { get; set; }

        public int NoDownvotes { get; set; }

        public DateTime CreatedAt { get; set; }

        public Student Author { get; set; }

        public Question Question { get; set; }
    }
}
