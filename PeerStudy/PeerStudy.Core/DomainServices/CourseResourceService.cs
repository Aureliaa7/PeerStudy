using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Emails;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class CourseResourceService : ResourceBaseService<CourseResourceDetailsModel>, ICourseResourceService
    {
        private readonly IGoogleDrivePermissionService drivePermissionService;
        private readonly IEmailService emailService;

        public CourseResourceService(
            IUnitOfWork unitOfWork,
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService drivePermissionService,
            IEmailService emailService) : base(fileService, unitOfWork)
        {
            this.drivePermissionService = drivePermissionService;
            this.emailService = emailService;
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

        public Task<List<CourseResourceDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            return GetAllAsync(courseId);
        }

        public async Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(UploadCourseResourcesModel data)
        {
            var parentFolderId = await GetParentFolderIdAsync(data.CourseId);
            var updatedData = SetParentFolderId(data.Resources, parentFolderId);
            var filesDetails = await UploadFilesAsync(updatedData);
            string ownerName = await GetOwnerNameAsync(data.OwnerId);

            var courseResources = new List<CourseResource>();
            var resourceDetailsModels = new List<CourseResourceDetailsModel>();

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
                    Type = Enums.ResourceType.File,
                    CourseUnitId = data.CourseUnitId
                });

                resourceDetailsModels.Add(new CourseResourceDetailsModel
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

            await NotifyStudentsAsync(data.CourseId, createdResources.Select(x => x.FileName).ToList());

            return SetResourcesIds(driveFileIdDbResourceIdPairs, resourceDetailsModels);
        }

        private async Task NotifyStudentsAsync(Guid courseId, List<string> resourcesNames)
        {
            try
            {
                var course = await unitOfWork.CoursesRepository.GetFirstOrDefaultAsync(x => x.Id == courseId,
                    includeProperties: nameof(Teacher)) ?? throw new EntityNotFoundException($"Course with id {courseId} was not found!");

                var studentsEmails = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.CourseId == courseId))
                    .Select(x => x.Student.Email)
                    .ToList();

                var emailModel = new NewCourseResourceEmailModel
                {
                    CourseTitle = course.Title,
                    EmailType = EmailType.NewCourseMaterial,
                    RecipientName = string.Empty,
                    TeacherName = $"{course.Teacher.FirstName} {course.Teacher.LastName}",
                    To = studentsEmails,
                    ResourceName = string.Join(", ", resourcesNames)
                };

                await emailService.SendAsync(emailModel);
            }
            catch (Exception ex)
            {
                //ToDo: log ex
            }
        }

        private async Task SetReadPermissionsForEnrolledStudentsAsync(Guid courseId, List<string> fileIds)
        {
            var studentsEmails = (await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.CourseId == courseId))
                .Select(x => x.Student.Email)
                .ToList();

            await drivePermissionService.SetPermissionsAsync(fileIds, studentsEmails, "reader");
        }

        protected override async Task<List<CourseResourceDetailsModel>> GetAsync(Guid id)
        {
            bool courseExist = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == id);
            if (!courseExist)
            {
                throw new EntityNotFoundException($"Cannot retrieve resources for course with id {id}. The course does not exist...");
            }

            var resources = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => x.CourseId == id,
                trackChanges: false))
                .Select(x => new CourseResourceDetailsModel
                {
                    CreatedAt = x.CreatedAt,
                    FileDriveId  =x.DriveFileId,
                    OwnerId = x.OwnerId,
                    FileName = x.FileName,
                    Id = x.Id,
                    OwnerName = $"{x.Owner.FirstName} {x.Owner.LastName}",
                    CouseUnitId = x.CourseUnitId
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

        public async Task DeleteRangeAsync(List<Guid> resourcesIds)
        {
            var driveFilesIds = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => resourcesIds.Contains(x.Id)))
                .Select(x => x.DriveFileId)
                .ToList();

            await fileService.DeleteRangeAsync(driveFilesIds);

            foreach (var id in resourcesIds)
            {
                await unitOfWork.CourseResourcesRepository.RemoveAsync(id);
            }
            await unitOfWork.SaveChangesAsync();
        }
    }
}
