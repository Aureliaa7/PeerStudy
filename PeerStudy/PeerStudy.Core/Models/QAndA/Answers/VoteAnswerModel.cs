using PeerStudy.Core.Enums;
using System;

namespace PeerStudy.Core.Models.QAndA.Answers
{
    public class VoteAnswerModel
    {
        public Guid AnswerId { get; set; }

        public Guid UserId { get; set; }

        public VoteType VoteType { get; set; }
    }
}
