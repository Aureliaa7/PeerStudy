namespace PeerStudy.Core.Models.QAndA.Questions
{
    public class FlatQuestionModel : QuestionModel
    {
        public int NoAnswers { get; set; }

        public string AuthorName { get; set; }

        public string ProfilePhotoName { get; set; }
    }
}
