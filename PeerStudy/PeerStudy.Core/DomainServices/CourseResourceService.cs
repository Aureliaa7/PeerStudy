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
    public class CourseResourceService : ResourceBaseService, ICourseResourceService
    {
        private readonly IGoogleDrivePermissionService drivePermissionService;

        public CourseResourceService(
            IUnitOfWork unitOfWork,
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService drivePermissionService) : base(fileService, unitOfWork)
        {
            this.drivePermissionService = drivePermissionService;
        }

        public async Task DeleteAsync(Guid resourceId)
        {
            var resource = await unitOfWork.CourseResourcesRepository.GetFirstOrDefaultAsync(x => x.Id == resourceId);
            if (resource == null)
            {
                throw new EntityNotFoundException($"Resource with id {resourceId} was not found!");
            }

            await fileService.DeleteAsync(resource.DriveFileId);
            await unitOfWork.CourseResourcesRepository.RemoveAsync(resourceId);
            await unitOfWork.SaveChangesAsync();
        }

        public Task<List<ResourceDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            return GetAllAsync(courseId);
        }

        public async Task<List<ResourceDetailsModel>> UploadResourcesAsync(UploadCourseResourcesModel data)
        {
            var parentFolderId = await GetParentFolderIdAsync(data.CourseId);
            var updatedData = SetParentFolderId(data.Resources, parentFolderId);
            var filesDetails = await UploadFilesAsync(updatedData);
            string ownerName = await GetOwnerNameAsync(data.OwnerId);

            var courseResources = new List<CourseResource>();
            var resourceDetailsModels = new List<ResourceDetailsModel>();

            foreach (var file in filesDetails)
            {
                var createdAt = DateTime.UtcNow;

                courseResources.Add(new CourseResource
                {
                    CreatedAt = createdAt,
                    CourseId = data.CourseId,
                    DriveFileId = file.FileDriveId,
                    FileName = file.Name,
                    OwnerId = data.OwnerId,
                    Type = Enums.ResourceType.File
                });

                resourceDetailsModels.Add(new ResourceDetailsModel
                {
                    CreatedAt = createdAt,
                    FileDriveId = file.FileDriveId,
                    FileName = file.Name,
                    IconLink = file.IconLink,
                    OwnerId = data.OwnerId,
                    OwnerName = ownerName,
                    WebViewLink = file.WebViewLink
                });
            }

            await SetReadPermissionsForEnrolledStudentsAsync(data.CourseId,
               courseResources.Select(x => x.DriveFileId).ToList());

            var createdResources = await unitOfWork.CourseResourcesRepository.AddRangeAsync(courseResources);
            await unitOfWork.SaveChangesAsync();

            var driveFileIdDbResourceIdPairs = createdResources.Select(x => new
            {
                x.DriveFileId,
                x.Id
            })
            .ToDictionary(x => x.DriveFileId, y => y.Id);

            return SetResourcesIds(driveFileIdDbResourceIdPairs, resourceDetailsModels);
        }


        private async Task SetReadPermissionsForEnrolledStudentsAsync(Guid courseId, List<string> fileIds)
        {
            var studentsEmails = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.CourseId == courseId))
                .Select(x => x.Student.Email)
                .ToList();

            await drivePermissionService.SetPermissionsAsync(fileIds, studentsEmails, "reader");
        }

        protected override async Task<List<ResourceDetailsModel>> GetAsync(Guid id)
        {
            bool courseExist = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == id);
            if (!courseExist)
            {
                throw new EntityNotFoundException($"Cannot retrieve resources for course with id {id}. The course does not exist...");
            }

            var resources = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => x.CourseId == id,
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}", trackChanges: false))
                .Select(x => new ResourceDetailsModel
                {
                    CreatedAt = x.CreatedAt,
                    FileDriveId  =x.DriveFileId,
                    OwnerId = x.OwnerId,
                    FileName = x.FileName,
                    Id = x.Id,
                    OwnerName = $"{x.Owner.FirstName} {x.Owner.LastName}"
                })
                .ToList();

            return resources;
        }

        protected override async Task<string> GetParentFolderIdAsync(Guid id)
        {
            var folderId = (await unitOfWork.CoursesRepository.GetAllAsync(x => x.Id == id, trackChanges: false))
              .Select(x => x.ResourcesDriveFolderId)
              .FirstOrDefault();

            return folderId;
        }
    }
}
