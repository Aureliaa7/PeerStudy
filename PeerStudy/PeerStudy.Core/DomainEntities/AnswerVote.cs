using System;

namespace PeerStudy.Core.DomainEntities
{
    public class AnswerVote : Vote
    {
        public Guid AnswerId { get; set; }

        public Answer Answer { get; set; }
    }
}
