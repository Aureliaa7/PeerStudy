using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.GoogleDriveModels;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public abstract class ResourceBaseService<T> where T: ResourceDetailsModel, new()
    {
        protected readonly IGoogleDriveFileService fileService;
        protected readonly IUnitOfWork unitOfWork;

        public ResourceBaseService(IGoogleDriveFileService fileService, IUnitOfWork unitOfWork)
        {
            this.fileService = fileService;
            this.unitOfWork = unitOfWork;
        }

        protected async Task<List<T>> GetAllAsync(Guid id)
        {
            var resources = await GetAsync(id);

            var fileIds = resources.Select(x => x.FileDriveId).ToList();
            var fileDetails = await fileService.GetFilesDetailsAsync(fileIds);

            return AddDriveDetailsToResourceModels(resources, fileDetails);
        }

        protected abstract Task<List<T>> GetAsync(Guid id);

        protected abstract Task<string> GetParentFolderIdAsync(Guid id);

        protected static List<UploadDriveFileModel> SetParentFolderId(List<UploadDriveFileModel> resources, string parentFolderId)
        {
            foreach (var resource in resources)
            {
                resource.ParentFolderId = parentFolderId;
            }

            return resources;
        }

        protected async Task<List<DriveFileDetailsModel>> UploadFilesAsync(List<UploadDriveFileModel> files)
        {
            var uploadedFilesDetails = new List<DriveFileDetailsModel>();

            foreach (var file in files)
            {
                try
                {
                    var googleDriveFildeDetails = await fileService.UploadFileAsync(file);
                    uploadedFilesDetails.Add(googleDriveFildeDetails);
                }
                catch (Exception ex)
                {
                    //TODO: log ex
                }
            }

            return uploadedFilesDetails;
        }

        protected async Task<string> GetOwnerNameAsync(Guid id)
        {
            var owner = await unitOfWork.UsersRepository.GetByIdAsync(id);
            if (owner == null)
            {
                throw new EntityNotFoundException($"User with id {id} was not found!");
            }

            return $"{owner.FirstName} {owner.LastName}";
        }

        private static List<T> AddDriveDetailsToResourceModels(List<T> resources, IDictionary<string, DriveFileDetailsModel> driveFileDetails)
        {
            foreach (var resource in resources)
            {
                if (driveFileDetails.TryGetValue(resource.FileDriveId, out var fileDriveDetails))
                {
                    resource.WebViewLink = fileDriveDetails.WebViewLink;
                    resource.IconLink = fileDriveDetails.IconLink;
                }
            }

            return resources;
        }

        protected static List<T> SetResourcesIds(Dictionary<string, Guid> fileIdResourceIdPairs, List<T> resources)
        {
            foreach (var resource in resources)
            {
                if (fileIdResourceIdPairs.TryGetValue(resource.FileDriveId, out Guid resourceId))
                {
                    resource.Id = resourceId;
                }
            }

            return resources;
        }
    }
}
