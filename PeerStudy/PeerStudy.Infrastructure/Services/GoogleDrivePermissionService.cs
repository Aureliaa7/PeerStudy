using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using PeerStudy.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class GoogleDrivePermissionService : GoogleDriveBaseService, IGoogleDrivePermissionService
    {
        private DriveService driveService;

        public GoogleDrivePermissionService(IConfigurationService configuration) : base(configuration)
        {
            driveService = GetDriveService();
        }

        public async Task SetPermissionsAsync(List<string> fileIds, List<string> emails, string role)
        {
            try
            {
                var batch = new BatchRequest(driveService);

                foreach (var fileId in fileIds)
                {
                    foreach (string email in emails)
                    {
                        Permission userPermission = new Permission
                        {
                            Type = "user",
                            Role = role,
                            EmailAddress = email
                        };

                        var request = driveService.Permissions.Create(userPermission, fileId);
                        batch.Queue(request, (Permission permission, RequestError error, int index, HttpResponseMessage message) =>
                        {
                            if (error != null)
                            {
                                // TODO: log the err
                            }
                        });
                    }
                }

                await batch.ExecuteAsync();
            }
            catch (Exception e)
            {
                //TODO: log the exception
            }
        }
    }
}
