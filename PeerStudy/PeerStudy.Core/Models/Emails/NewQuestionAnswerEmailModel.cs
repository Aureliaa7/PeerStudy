namespace PeerStudy.Core.Models.Emails
{
    public class NewQuestionAnswerEmailModel : EmailBaseModel
    {
        public string QuestionTitle { get; set; }

        public string AnswerAuthorName { get; set; }
    }
}
