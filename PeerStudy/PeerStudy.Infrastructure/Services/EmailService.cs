using MailKit.Net.Smtp;
using MimeKit;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models.Emails;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateService emailTemplateService;
        private readonly IEmailTemplateBodyService emailTemplateBodyService;
        private readonly EmailConfiguration emailConfiguration;

        public EmailService(IEmailTemplateService emailTemplateService,
            IEmailTemplateBodyService emailTemplateBodyService,
            EmailConfiguration emailConfiguration)
        {
            this.emailTemplateService = emailTemplateService;
            this.emailTemplateBodyService = emailTemplateBodyService;
            this.emailConfiguration = emailConfiguration;
        }

        public async Task SendAsync(EmailBaseModel emailModel)
        {
            var emailTemplate = await emailTemplateService.GetByTypeAsync(emailModel.EmailType);
            var updatedBodyContent = emailTemplateBodyService.ReplaceTokens(emailModel, emailTemplate.Body);
            var emailMessage = CreateEmailMessage(emailTemplate.Subject, updatedBodyContent, emailModel.To);
            
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(string subject, string emailBody, List<string> emails)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(emailConfiguration.From));

            foreach (var email in emails)
            {
                emailMessage.To.Add(MailboxAddress.Parse(email));
            }
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = emailBody };

            return emailMessage;
        }
        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfiguration.SmtpServer, emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfiguration.UserName, emailConfiguration.Password);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    //ToDo: log
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
