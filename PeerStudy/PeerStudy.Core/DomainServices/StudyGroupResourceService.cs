using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class StudyGroupResourceService : ResourceBaseService<ResourceDetailsModel>, IStudyGroupResourceService
    {
        private readonly IGoogleDrivePermissionService drivePermissionService;

        public StudyGroupResourceService(
            IUnitOfWork unitOfWork, 
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService drivePermissionService) : base(fileService, unitOfWork)
        {
            this.drivePermissionService = drivePermissionService;
        }

        public Task<List<ResourceDetailsModel>> GetByStudyGroupIdAsync(Guid id)
        {
           return GetAllAsync(id);
        }

        public async Task<List<ResourceDetailsModel>> UploadAsync(UploadStudyGroupResourceModel uploadResourceModel)
        {
            var parentFolderId = await GetParentFolderIdAsync(uploadResourceModel.StudyGroupId);

            var updatedData = SetParentFolderId(uploadResourceModel.Resources, parentFolderId);
            var filesDetails = await UploadFilesAsync(updatedData);
            string ownerName = await GetOwnerNameAsync(uploadResourceModel.OwnerId);

            var studyGroupFiles = new List<StudyGroupFile>();
            var resourceDetailsModels = new List<ResourceDetailsModel>();

            foreach (var file in filesDetails)
            {
                var createdAt = DateTime.UtcNow;

                studyGroupFiles.Add(new StudyGroupFile
                {
                    CreatedAt = createdAt,
                    DriveFileId = file.FileDriveId,
                    FileName = file.Name,
                    OwnerId = uploadResourceModel.OwnerId,
                    StudyGroupId = uploadResourceModel.StudyGroupId
                });

                resourceDetailsModels.Add(new ResourceDetailsModel
                {
                    CreatedAt = createdAt,
                    FileDriveId = file.FileDriveId,
                    FileName = file.Name,
                    IconLink = file.IconLink,
                    OwnerId = uploadResourceModel.OwnerId,
                    OwnerName = ownerName,
                    WebViewLink = file.WebViewLink
                });
            }

            await SetReadPermissionsAsync(uploadResourceModel.StudyGroupId,
               studyGroupFiles.Select(x => x.DriveFileId).ToList());

            var createdResources = await unitOfWork.StudyGroupFilesRepository.AddRangeAsync(studyGroupFiles);
            await unitOfWork.SaveChangesAsync();

            var driveFileIdDbResourceIdPairs = createdResources.Select(x => new
            {
                x.DriveFileId,
                x.Id
            })
            .ToDictionary(x => x.DriveFileId, y => y.Id);

            return SetResourcesIds(driveFileIdDbResourceIdPairs, resourceDetailsModels);
        }

        private async Task SetReadPermissionsAsync(Guid studyGroupId, List<string> fileIds)
        {
            var usersEmails = (await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.Id == studyGroupId))
                .SelectMany(x => x.StudentStudyGroups.Select(x => x.Student.Email))
                .ToList();

            var studyGroup = (await unitOfWork.StudyGroupRepository.GetFirstOrDefaultAsync(x => x.Id == studyGroupId,
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}"));

            if (studyGroup != null && studyGroup.Course?.Teacher != null)
            {
                usersEmails.Add(studyGroup.Course.Teacher.Email);
            }

            await drivePermissionService.SetPermissionsAsync(fileIds, usersEmails, "reader");
        }

        protected override async Task<List<ResourceDetailsModel>> GetAsync(Guid id)
        {
            var resources = (await unitOfWork.StudyGroupFilesRepository.GetAllAsync(x =>
           x.StudyGroupId == id))
           .Select(x => new ResourceDetailsModel
           {
               CreatedAt = x.CreatedAt,
               FileDriveId = x.DriveFileId,
               FileName = x.FileName,
               OwnerId = x.OwnerId,
               OwnerName = $"{x.Owner.FirstName} {x.Owner.LastName}",
               Id = x.Id
           })
           .ToList();

            return resources;
        }

        protected override async Task<string> GetParentFolderIdAsync(Guid id)
        {
            var folderId = (await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.Id == id, trackChanges: false))
              .Select(x => x.DriveFolderId)
              .FirstOrDefault();

            return folderId;
        }

        public async Task DeleteAsync(Guid id)
        {
            var resource = await unitOfWork.StudyGroupFilesRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                throw new EntityNotFoundException($"Resource with id {id} was not found!");
            }

            await fileService.DeleteAsync(resource.DriveFileId);
            await unitOfWork.StudyGroupFilesRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
