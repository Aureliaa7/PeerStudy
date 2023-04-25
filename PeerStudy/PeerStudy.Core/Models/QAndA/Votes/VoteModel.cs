using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.Models.QAndA.Votes
{
    public class VoteModel
    {
        public Guid EntityId { get; set; }

        public Guid UserId { get; set; }

        public VoteType VoteType { get; set; }
    }
}
