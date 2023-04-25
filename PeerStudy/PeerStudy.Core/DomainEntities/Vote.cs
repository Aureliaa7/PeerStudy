using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.DomainEntities
{
    public class Vote
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public VoteType VoteType { get; set; }

        public Student Author { get; set; }
    }
}
