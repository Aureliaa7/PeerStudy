using Google.Apis.Drive.v3;
using PeerStudy.Core.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class GoogleDriveFileService : GoogleDriveBaseService, IGoogleDriveFileService
    {
        private DriveService driveService;

        private const string folderMimeType = "application/vnd.google-apps.folder";
        private const string fileMimeType = "application/vnd.google-apps.file";

        public GoogleDriveFileService(IConfigurationService configuration) : base (configuration)
        {
            driveService = GetDriveService();
        }

        public async Task<string> CreateFolderAsync(string folderName, string parent = null)
        {
            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folderName;
            driveFolder.MimeType = folderMimeType;
            if (!string.IsNullOrEmpty(parent))
            {
                driveFolder.Parents = new string[] { parent };
            }
            var command = driveService.Files.Create(driveFolder);
            var file = await command.ExecuteAsync();

            return file.Id;
        }

        public Task<string> UploadFileAsync(string parentId, Stream file, string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
