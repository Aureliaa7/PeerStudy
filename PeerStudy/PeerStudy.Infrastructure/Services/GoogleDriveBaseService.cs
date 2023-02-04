using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using PeerStudy.Core.Interfaces.Services;

namespace PeerStudy.Infrastructure.Services
{
    public class GoogleDriveBaseService
    {
        private readonly IConfigurationService configuration;

        public GoogleDriveBaseService(IConfigurationService configuration)
        {
            this.configuration = configuration;
        }

        protected DriveService GetDriveService()
        {
            var credential = GoogleCredential.FromFile(configuration.GoogleDriveCredentialsPath)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            return service;
        }
    }
}
