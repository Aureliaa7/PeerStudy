using PeerStudy.Core.DomainEntities;
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
    public class CourseResourceService : ICourseResourceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGoogleDriveFileService fileService;

        public CourseResourceService(IUnitOfWork unitOfWork, IGoogleDriveFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
        }

        public async Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(List<UploadCourseResourceModel> resources)
        {
           var updatedResourcesModels = await GetUpdatedResourceModelsAsync(resources);

            var courseResources = new List<CourseResource>();   

            foreach (var resource in updatedResourcesModels)
            {
                try
                {
                    string fileId = await fileService.UploadFileAsync(resource);
                    courseResources.Add(new CourseResource
                    {
                        CreatedAt = DateTime.UtcNow,
                        CourseId = resource.CourseId,
                        DriveFileId = fileId,
                        Title = resource.Name,
                        Type = Enums.ResourceType.File
                    });
                }
                catch (Exception ex)
                {
                    //TODO: log ex

                }
            }

            var savedResources = await unitOfWork.CourseResourcesRepository.AddRangeAsync(courseResources);
            await unitOfWork.SaveChangesAsync();

            return MapToCourseResourceDetailsModels(savedResources);
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

        private List<CourseResourceDetailsModel> MapToCourseResourceDetailsModels(List<CourseResource> resources)
        {
            var resourcesModels = new List<CourseResourceDetailsModel>();
            foreach (var resource in resources)
            {
                resourcesModels.Add(new CourseResourceDetailsModel
                {
                    CreatedAt = resource.CreatedAt,
                    Id = resource.Id,
                    Title = resource.Title
                });
            }

            return resourcesModels;
        }
    }
}
