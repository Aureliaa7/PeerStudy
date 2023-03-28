using Google.Apis.Drive.v3;
using Google.Apis.Requests;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models.GoogleDriveModels;
using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

        public async Task<DriveFileDetailsModel> UploadFileAsync(UploadDriveFileModel model)
        {
            using (var ms = new MemoryStream(model.FileContent))
            {
                var driveFile = new DriveFile();
                driveFile.Name = model.Name;
                driveFile.MimeType = fileMimeType;
                driveFile.Parents = new string[] { model.ParentFolderId };

                var request = driveService.Files.Create(driveFile, ms, fileMimeType);
                request.Fields = "*";

                var response = await request.UploadAsync();
                if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                {
                    throw response.Exception;
                }

                //Note: add app email as writer for validation purposes
                await permissionService.SetPermissionsAsync(new List<string> { request.ResponseBody.Id }, new List<string> { model.OwnerEmail, configuration.AppEmail }, writerRole);

                return new DriveFileDetailsModel
                {
                    FileDriveId = request.ResponseBody.Id,
                    IconLink = request.ResponseBody.IconLink,
                    Name = request.ResponseBody.Name,
                    WebViewLink = request.ResponseBody.WebViewLink
                };
            }
        }

        public async Task<Dictionary<string, DriveFileDetailsModel>> GetFilesDetailsAsync(List<string> fileIds)
        {
            var fileDetailsPairs = new Dictionary<string, DriveFileDetailsModel>();

            BatchRequest.OnResponse<DriveFile> callback = delegate (
                   DriveFile fileDetails,
                   RequestError error,
                   int index,
                   HttpResponseMessage message)
            {
                if (error != null)
                {
                    // TODO: log the err
                }
                else
                {
                    fileDetailsPairs.Add(fileDetails.Id, MapToFileDetailsModel(fileDetails));
                }
            };


            var batchRequest = new BatchRequest(driveService);
            foreach (var fileId in fileIds)
            {
                var request = driveService.Files.Get(fileId);
                request.Fields = "*"; //TODO: request only the needed props
                batchRequest.Queue(request, callback);
            }

            await batchRequest.ExecuteAsync();

            return fileDetailsPairs;
        }

        private DriveFileDetailsModel MapToFileDetailsModel(DriveFile file)
        {
            return new DriveFileDetailsModel
            {
                FileDriveId = file.Id,
                Name = file.Name,
                WebViewLink = file.WebViewLink,
                IconLink = file.IconLink
            };
        }

        public async Task DeleteAsync(string resourceId)
        {
            var deleteRequest = driveService.Files.Delete(resourceId);
            await deleteRequest.ExecuteAsync();
        }

        public async Task DeleteRangeAsync(List<string> fileIds)
        {
            var batchRequest = new BatchRequest(driveService);
            foreach (var fileId in fileIds)
            {
                var request = driveService.Files.Delete(fileId);
                batchRequest.Queue<DriveFile>(request, (content, error, i, message) => {
                    if (error != null)
                    {
                        // log err
                    }
                });
            }

            await batchRequest.ExecuteAsync();
        }
    }
}
