using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.GoogleDriveModels;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AssignmentFileService : IAssignmentFileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGoogleDriveFileService fileService;
        private readonly IGoogleDrivePermissionService drivePermissionService;

        public AssignmentFileService(
            IUnitOfWork unitOfWork,
            IGoogleDriveFileService fileService,
            IGoogleDrivePermissionService drivePermissionService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
            this.drivePermissionService = drivePermissionService;
        }

        public async Task<AssignmentFilesModel> GetUploadedFilesByStudyGroupAsync(Guid assignmentId, Guid studyGroupId)
        {
            var assignment = await unitOfWork.AssignmentsRepository.GetFirstOrDefaultAsync(x =>
            x.Id == assignmentId && x.StudyGroupId == studyGroupId) ?? throw new EntityNotFoundException($"StudentAssignment with assignmentId {assignmentId} and studyGroupId {studyGroupId} was not found!");

            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x => x.AssignmentId ==
            assignmentId && x.StudyGroupId == studyGroupId);

            var assignmentFilesModel = new AssignmentFilesModel
            {
                Id = assignmentId,
                Deadline = assignment.Deadline,
                Description = assignment.Description,
                Title = assignment.Title,
                Points = studentAssignment?.Points ?? null,
                CompletedAt = assignment.CompletedAt,
                StudentGroupId = assignment.StudyGroupId
            };

            var studyGroupAssignmentFiles = (await unitOfWork.StudyGroupAssignmentFilesRepository.GetAllAsync(x =>
                x.AssignmentId == assignmentId && x.Assignment.StudyGroupId == studyGroupId))
            .ToList();

            var fileIds = studyGroupAssignmentFiles.Select(x => x.DriveFileId).ToList();
            var filesDetails = await fileService.GetFilesDetailsAsync(fileIds);
            assignmentFilesModel.StudyGroupAssignmentFiles = MapToAssignmentFileDetailsModels(studyGroupAssignmentFiles, filesDetails);

            return assignmentFilesModel;
        }

        public async Task<List<StudyGroupAssignmentFileModel>> UploadWorkAsync(UploadAssignmentFilesModel model, DateTime completedAt)
        {
            var assignment = await unitOfWork.AssignmentsRepository.GetFirstOrDefaultAsync(x => x.Id ==
            model.AssignmentId && x.StudyGroupId == model.StudyGroupId) ?? throw new EntityNotFoundException($"Assignment with assignment id {model.AssignmentId} and study group id {model.StudyGroupId} was not found!");

            var (teacherEmail, driveFolderId) = await GetTeacherEmailDriveFolderIdAsync(model.AssignmentId);
            var studentIdsEmails = await GetStudentIdEmailPairsAsync(model.StudyGroupId);
            string ownerEmail = studentIdsEmails.First(x => x.Key == model.OwnerId).Value;

            var uploadedFilesDetails = await SaveFilesAsync(
                model.Files,
                ownerEmail,
                driveFolderId);

            await SetPemissionsAsync(
                uploadedFilesDetails
                    .Select(x => x.Value.FileDriveId)
                    .ToList(),
                teacherEmail,
                studentIdsEmails
                    .Where(x => x.Key != model.OwnerId)
                    .Select(x => x.Value)
                    .ToList());

            var savedFiles = await SaveToDBStudyGroupAssignmentFilesAsync(uploadedFilesDetails, model.AssignmentId, completedAt, model.OwnerId);

            assignment.CompletedAt = completedAt;
            await unitOfWork.AssignmentsRepository.UpdateAsync(assignment);
            await unitOfWork.SaveChangesAsync();

            return MapToAssignmentFileDetailsModels(savedFiles, uploadedFilesDetails);
        }

        private async Task<(string teacherEmail, string driveFolderId)> GetTeacherEmailDriveFolderIdAsync(Guid assignmentId)
        {
            var teacherEmailDriveFolderId = (await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.Id == assignmentId))
               .Select(x => new
               {
                   TeacherEmail = x.CourseUnit.Course.Teacher.Email,
                   x.CourseUnit.Course.AssignmentsDriveFolderId
               })
               .FirstOrDefault();

            return teacherEmailDriveFolderId == null
                ? throw new EntityNotFoundException($"Could not find details for assignment with id {assignmentId}!")
                : (teacherEmailDriveFolderId.TeacherEmail, teacherEmailDriveFolderId.AssignmentsDriveFolderId);
        }

        private async Task<Dictionary<string, DriveFileDetailsModel>> SaveFilesAsync(
            List<UploadFileModel> files,
            string ownerEmail,
            string driveFolderId)
        {
            var uploadedFilesDetails = new Dictionary<string, DriveFileDetailsModel>();

            foreach (var file in files)
            {
                try
                {
                    var googleDriveFileDetails = await fileService.UploadFileAsync(new UploadDriveFileModel
                    {
                        FileContent = file.FileContent,
                        Name = file.Name,
                        OwnerEmail = ownerEmail,
                        ParentFolderId = driveFolderId
                    });

                    uploadedFilesDetails.Add(googleDriveFileDetails.FileDriveId, googleDriveFileDetails);
                }
                catch (Exception ex)
                {
                    //TODO: log ex
                }
            }

            return uploadedFilesDetails;
        }

        private async Task SetPemissionsAsync(List<string> filesIds, string teacherEmail, List<string> studyGroupMembersEmails)
        {
            await drivePermissionService.SetPermissionsAsync(filesIds,new List<string> { teacherEmail },"reader");

            await drivePermissionService.SetPermissionsAsync(filesIds, studyGroupMembersEmails,"edit");
        }

        private async Task<List<StudyGroupAssignmentFile>> SaveToDBStudyGroupAssignmentFilesAsync(
            Dictionary<string, DriveFileDetailsModel> uploadedFilesDetails,
            Guid assignmentId,
            DateTime completedAt,
            Guid ownerId)
        {
            var studyGroupAssignmentFiles = new List<StudyGroupAssignmentFile>();

            foreach (var pair in uploadedFilesDetails)
            {
                studyGroupAssignmentFiles.Add(new StudyGroupAssignmentFile
                {
                    AssignmentId = assignmentId,
                    CreatedAt = completedAt,
                    DriveFileId = pair.Value.FileDriveId,
                    OwnerId = ownerId,
                    FileName = pair.Value.Name
                });
            }

            var savedFiles = await unitOfWork.StudyGroupAssignmentFilesRepository.AddRangeAsync(studyGroupAssignmentFiles);
            await unitOfWork.SaveChangesAsync();

            return savedFiles;
        }

        private async Task<Dictionary<Guid, string>> GetStudentIdEmailPairsAsync(Guid studyGroupId)
        {
            var studentIdsEmails = (await unitOfWork.StudyGroupRepository.GetAllAsync(x => x.Id == studyGroupId))
             .SelectMany(x => x.StudentStudyGroups)
             .Select(x => new
             {
                 Id = x.StudentId,
                 x.Student.Email
             })
             .ToList();

            return studentIdsEmails.ToDictionary(x => x.Id, x => x.Email);
        }

        private List<StudyGroupAssignmentFileModel> MapToAssignmentFileDetailsModels(List<StudyGroupAssignmentFile> resources, IDictionary<string, DriveFileDetailsModel> driveFileDetails)
        {
            var resourcesModels = new List<StudyGroupAssignmentFileModel>();
            foreach (var resource in resources)
            {
                if (driveFileDetails.TryGetValue(resource.DriveFileId, out var fileDriveDetails))
                {
                    resourcesModels.Add(new StudyGroupAssignmentFileModel
                    {
                        Id = resource.Id,
                        IconLink = fileDriveDetails.IconLink,
                        WebViewLink = fileDriveDetails.WebViewLink,
                        FileDriveId = fileDriveDetails.FileDriveId,
                        Name = fileDriveDetails.Name
                    });
                }
            }

            return resourcesModels;
        }

        public async Task DeleteAsync(string driveFileId, Guid studentAssignmentFileId)
        {
            await fileService.DeleteAsync(driveFileId);

            await unitOfWork.StudyGroupAssignmentFilesRepository.RemoveAsync(studentAssignmentFileId);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
