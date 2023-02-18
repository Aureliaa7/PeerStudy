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

        public CourseResourceService(IUnitOfWork unitOfWork, IGoogleDriveFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
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

        public async Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(List<UploadCourseResourceModel> resources)
        {
           var updatedResourcesModels = await GetUpdatedResourceModelsAsync(resources);

            var courseResources = new List<CourseResource>();   

            var uploadedFilesDetails = new Dictionary<string, FileDetailsModel>();

            foreach (var resource in updatedResourcesModels)
            {
                try
                {
                    var googleDriveFildeDetails = await fileService.UploadFileAsync(resource);
                    var createdAt = DateTime.UtcNow;
                    courseResources.Add(new CourseResource
                    {
                        CreatedAt = createdAt,
                        CourseId = resource.CourseId,
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

            //TODO: give students read permissions

            var createdResources = await unitOfWork.CourseResourcesRepository.AddRangeAsync(courseResources);
            await unitOfWork.SaveChangesAsync();

            var createdResourcesIds = createdResources.Select(x => x.Id).ToList();

            var savedResources = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => createdResourcesIds.Contains(x.Id),
                includeProperties: $"{nameof(Course)}.{nameof(Course.Teacher)}", trackChanges: false))
                .ToList();

            return MapToCourseResourceDetailsModels(savedResources, uploadedFilesDetails);
        }

        private async Task<List<UploadCourseResourceModel>> GetUpdatedResourceModelsAsync(List<UploadCourseResourceModel> resources)
        {
            var courseIds = resources.Select(x => x.CourseId).Distinct().ToList();

            var courseIdResourcesFolderIdPairs = (await unitOfWork.CoursesRepository.GetAllAsync(x => courseIds.Contains(x.Id), trackChanges: false))
                .Select(x => new
                {
                    CourseId = x.Id,
                    ResourcesFolderId = x.ResourcesDriveFolderId
                })
                .ToDictionary(x => x.CourseId, y => y.ResourcesFolderId);

            foreach (var resource in resources)
            {
                if (courseIdResourcesFolderIdPairs.TryGetValue(resource.CourseId, out var parentFolderId))
                {
                    resource.ParentFolderId = parentFolderId;
                }
            }

            return resources;
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
