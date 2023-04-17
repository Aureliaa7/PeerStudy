using PeerStudy.Core.Models.QAndA.Answers;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.QAndA.Questions
{
    public class QuestionDetailsModel : QuestionModel
    {
        public string HtmlDescription { get; set; }

        public List<AnswerDetailsModel> Answers { get; set; } = new List<AnswerDetailsModel>();
    }
}
