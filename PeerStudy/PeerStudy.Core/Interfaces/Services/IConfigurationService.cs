namespace PeerStudy.Core.Interfaces.Services
{
    public interface IConfigurationService
    {
        string GoogleDriveCredentialsPath { get; }

        string AppEmail { get; }

        string AppPassword { get; }

        string JWTKey { get; }
    }
}
