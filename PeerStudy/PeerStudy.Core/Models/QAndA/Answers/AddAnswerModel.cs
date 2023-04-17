using System;

namespace PeerStudy.Core.Models.QAndA.Answers
{
    public class AddAnswerModel
    {
        public Guid AuthorId { get; set; }

        public Guid QuestionId { get; set; }

        public string Content { get; set; }
    }
}
