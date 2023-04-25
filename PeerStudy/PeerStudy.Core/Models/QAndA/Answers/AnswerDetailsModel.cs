using System;
using System.Collections.Generic;
using PeerStudy.Core.Models.QAndA.Votes;

namespace PeerStudy.Core.Models.QAndA.Answers
{
    public class AnswerDetailsModel
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string HtmlContent { get; set; }

        public DateTime CreatedAt { get; set; }

        public int NoUpvotes { get; set; }

        public int NoDownvotes { get; set; }

        public List<VoteDetailsModel> Votes { get; set; } = new List<VoteDetailsModel>();

        //Flag used for updating an answer. It's needed by PeerStudyTextEditor component
        public bool IsReadOnly { get; set; } = true;
    }
}
