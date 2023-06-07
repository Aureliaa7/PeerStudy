using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Votes;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.QAndA.Questions
{
    public class QuestionDetailsModel : QuestionModel
    {
        public string HtmlDescription { get; set; }

        public string AuthorName { get; set; }

        public string AuthorProfileImageName { get; set; }

        public List<AnswerDetailsModel> Answers { get; set; } = new List<AnswerDetailsModel>();

        public int NoUpvotes { get; set; }

        public int NoDownvotes { get; set; }

        public List<VoteDetailsModel> Votes { get; set; } = new List<VoteDetailsModel>();
    }
}
