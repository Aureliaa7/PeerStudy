using Microsoft.Extensions.Configuration;
using PeerStudy.Core.Interfaces.Services;
using System;

namespace PeerStudy.Infrastructure.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GoogleDriveCredentialsPath => GetConfigValue(nameof(GoogleDriveCredentialsPath));

        public string AppEmail => GetConfigValue(nameof(AppEmail));

        public string AppPassword => GetConfigValue(nameof(AppPassword));

        public string JWTKey => GetConfigValue(nameof(JWTKey));

        public int MaxPostponedDeadlinesPerStudyGroup => GetNumber(nameof(MaxPostponedDeadlinesPerStudyGroup));

        public int NoDaysToPostponeDeadline => GetNumber(nameof(NoDaysToPostponeDeadline));

        private string GetConfigValue(string key)
        {
            try
            {
                return configuration.GetSection(key).Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private int GetNumber(string key)
        {
            try
            {
                return Convert.ToInt32(configuration.GetSection(key).Value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
