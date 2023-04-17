using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class AnswerVote
    {
        public Guid Id { get; set; }

        public Guid AnswerId { get; set; }

        public Guid UserId { get; set; }

        public VoteType Type { get; set; }

        public Answer Answer { get; set; }

        public Student User { get; set; }
    }
}
