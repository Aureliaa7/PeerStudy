using PeerStudy.Core.Models.Emails;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IEmailTemplateBodyService
    {
        string ReplaceTokens(EmailBaseModel emailModel, string emailBody);
    }
}
