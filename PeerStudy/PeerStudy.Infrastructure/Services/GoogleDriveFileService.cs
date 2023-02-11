using Google.Apis.Drive.v3;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace PeerStudy.Infrastructure.Services
{
    public class GoogleDriveFileService : GoogleDriveBaseService, IGoogleDriveFileService
    {
        private DriveService driveService;

        private readonly IGoogleDrivePermissionService permissionService;
        private readonly IConfigurationService configuration;

        private const string folderMimeType = "application/vnd.google-apps.folder";
        private const string fileMimeType = "application/vnd.google-apps.file";
        private const string writerRole = "writer";

        public GoogleDriveFileService(IConfigurationService configuration, IGoogleDrivePermissionService permissionService) : base (configuration)
        {
            driveService = GetDriveService();
            this.permissionService = permissionService;
            this.configuration = configuration;
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

        public async Task<string> UploadFileAsync(UploadFileModel model)
        {
            using (var ms = new MemoryStream(model.FileContent))
            {
                var driveFile = new DriveFile();
                driveFile.Name = model.Name;
                driveFile.MimeType = fileMimeType;
                driveFile.Parents = new string[] { model.ParentFolderId };

                var request = driveService.Files.Create(driveFile, ms, fileMimeType);
                request.Fields = "id";

                var response = await request.UploadAsync();
                if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                {
                    throw response.Exception;
                }

                //Note: add app email as writer for validation purposes
                await permissionService.SetPermissionsAsync(request.ResponseBody.Id, new List<string> { model.OwnerEmail, configuration.AppEmail }, writerRole);

                return request.ResponseBody.Id;
            }
        }
    }
}
