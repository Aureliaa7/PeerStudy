using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
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
    public class CourseResourceService : ICourseResourceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGoogleDriveFileService fileService;
        private readonly IGoogleDrivePermissionService drivePermissionService;

        public CourseResourceService(
            IUnitOfWork unitOfWork,
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService drivePermissionService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
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

        public async Task<List<CourseResourceDetailsModel>> GetAsync(Guid courseId)
        {
            bool courseExist = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == courseId);
            if (!courseExist)
            {
                throw new EntityNotFoundException($"Cannot retrieve resources for course with id {courseId}. The course does not exist...");
            }

            var resources = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => x.CourseId == courseId,
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}", trackChanges: false))
                .ToList();

            var fileIds = resources.Select(x => x.DriveFileId).ToList();

            var fileDetails = await fileService.GetFilesDetailsAsync(fileIds);

            return MapToCourseResourceDetailsModels(resources, fileDetails);
        }

        public async Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(UploadCourseResourcesModel resources)
        {
           var updatedData = await SetParentFolderIdAsync(resources);

            var (FilesDetails, CourseResources) = await UploadFilesAsync(updatedData);

            var createdResources = await unitOfWork.CourseResourcesRepository.AddRangeAsync(CourseResources);
            await unitOfWork.SaveChangesAsync();

            var createdResourcesIds = createdResources.Select(x => x.Id).ToList();

            var savedResources = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => createdResourcesIds.Contains(x.Id),
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}", trackChanges: false))
                .ToList();

            return MapToCourseResourceDetailsModels(savedResources, FilesDetails);
        }

        private async Task<(Dictionary<string, FileDetailsModel> FilesDetails, List<CourseResource> CourseResources)> 
            UploadFilesAsync(UploadCourseResourcesModel data)
        {
            var uploadedFilesDetails = new Dictionary<string, FileDetailsModel>();
            var courseResources = new List<CourseResource>();

            foreach (var resource in data.Resources)
            {
                try
                {
                    var googleDriveFildeDetails = await fileService.UploadFileAsync(resource);
                    var createdAt = DateTime.UtcNow;
                    courseResources.Add(new CourseResource
                    {
                        CreatedAt = createdAt,
                        CourseId = data.CourseId,
                        DriveFileId = googleDriveFildeDetails.FileDriveId,
                        Title = resource.Name,
                        Type = Enums.ResourceType.File
                    });
                    uploadedFilesDetails.Add(googleDriveFildeDetails.FileDriveId, googleDriveFildeDetails);
                }
                catch (Exception ex)
                {
                    //TODO: log ex
                }
            }

            await SetReadPermissionsForEnrolledStudentsAsync(data.CourseId,
               courseResources.Select(x => x.DriveFileId).ToList());

            return (uploadedFilesDetails, courseResources);
        }

        private async Task SetReadPermissionsForEnrolledStudentsAsync(Guid courseId, List<string> fileIds)
        {
            var studentsEmails = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.CourseId == courseId))
                .Select(x => x.Student.Email)
                .ToList();

            await drivePermissionService.SetPermissionsAsync(fileIds, studentsEmails, "reader");
        }

        private async Task<UploadCourseResourcesModel> SetParentFolderIdAsync(UploadCourseResourcesModel data)
        {
            var parentFolderId = (await unitOfWork.CoursesRepository.GetAllAsync(x => x.Id == data.CourseId, trackChanges: false))
                .Select(x => x.ResourcesDriveFolderId)
                .FirstOrDefault();

            foreach (var resource in data.Resources)
            {
                resource.ParentFolderId = parentFolderId;
            }

            return data;
        }

        private List<CourseResourceDetailsModel> MapToCourseResourceDetailsModels(List<CourseResource> resources, IDictionary<string, FileDetailsModel> driveFileDetails)
        {
            var resourcesModels = new List<CourseResourceDetailsModel>();
            foreach (var resource in resources)
            {
                if (driveFileDetails.TryGetValue(resource.DriveFileId, out var fileDriveDetails))
                {
                    resourcesModels.Add(new CourseResourceDetailsModel
                    {
                        CreatedAt = resource.CreatedAt,
                        Id = resource.Id,
                        Title = resource.Title,
                        IconLink = fileDriveDetails.IconLink,
                        TeacherName = $"{resource.Course?.Teacher?.FirstName} {resource.Course?.Teacher?.LastName}",
                        WebViewLink = fileDriveDetails.WebViewLink,
                        FileDriveId = fileDriveDetails.FileDriveId,
                        TeacherId = resource.Course.TeacherId
                    });
                }
            }

            return resourcesModels;
        }
    }
}
