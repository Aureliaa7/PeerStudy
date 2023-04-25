using System;

namespace PeerStudy.Core.DomainEntities
{
    public class QuestionVote: Vote
    {
        public Guid QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
