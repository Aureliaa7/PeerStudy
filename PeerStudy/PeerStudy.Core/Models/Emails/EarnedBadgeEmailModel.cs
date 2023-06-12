namespace PeerStudy.Core.Models.Emails
{
    public class EarnedBadgeEmailModel : EmailBaseModel
    {
        public string BadgeTitle { get; set; }

        public string BadgeDescription { get; set; }

        public int NoEarnedPoints { get; set; }
    }
}
