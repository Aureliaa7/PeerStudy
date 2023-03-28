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

        public async Task<AssignmentFilesModel> GetUploadedFilesByStudentAsync(Guid assignmentId, Guid studentId)
        {
            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x =>
            x.AssignmentId == assignmentId && x.StudentId == studentId, includeProperties: nameof(Assignment));

            if (studentAssignment == null)
            {
                throw new EntityNotFoundException($"StudentAssignment with assignmentId {assignmentId} and studentId {studentId} was not found!");
            }
            var assignmentFilesModel = new AssignmentFilesModel
            {
                AssignmentId = assignmentId,
                Deadline = studentAssignment.Assignment.Deadline,
                Description = studentAssignment.Assignment.Description,
                Title = studentAssignment.Assignment.Title,
                Points = studentAssignment.Points,
                CompletedAt = studentAssignment.CompletedAt,
                StudentAssignmentId = studentAssignment.Id
            };

            var studentAssignmentFiles = (await unitOfWork.StudentAssignmentFilesRepository.GetAllAsync(x =>
                x.StudentAssignment.AssignmentId == assignmentId && x.StudentAssignment.StudentId == studentId, includeProperties: nameof(StudentAssignment)))
            .ToList();

            var fileIds = studentAssignmentFiles.Select(x => x.DriveFileId).ToList();
            var filesDetails = await fileService.GetFilesDetailsAsync(fileIds);
            assignmentFilesModel.StudentAssignmentFiles = MapToAssignmentFileDetailsModels(studentAssignmentFiles, filesDetails);

            return assignmentFilesModel;
        }

        public async Task<List<StudentAssignmentFileModel>> UploadWorkAsync(UploadAssignmentFilesModel model, DateTime completedAt)
        {
            var studentAssignment = await unitOfWork.StudentAssignmentsRepository.GetFirstOrDefaultAsync(x => x.AssignmentId ==
            model.AssignmentId && x.StudentId == model.StudentId, includeProperties: $"{nameof(Assignment)},{nameof(Student)}");

            if (studentAssignment == null)
            {
                throw new EntityNotFoundException($"Student assignment with assignment id {model.AssignmentId} and student id {model.StudentId} was not found!");
            }

            var assignmentDetails = (await unitOfWork.AssignmentsRepository.GetAllAsync(x => x.Id == model.AssignmentId))
                .Select(x => new
                {
                    TeacherEmail = x.Course.Teacher.Email,
                    AssignmentsDriveFolderId = x.Course.AssignmentsDriveFolderId
                })
                .FirstOrDefault();

            if (assignmentDetails == null)
            {
                throw new EntityNotFoundException($"Could not find details for assignment with id {model.AssignmentId}!");
            }

            return await SaveFilesAsync(
                model.Files, 
                studentAssignment, 
                assignmentDetails.AssignmentsDriveFolderId, 
                completedAt, 
                assignmentDetails.TeacherEmail);
        }

        private async Task<List<StudentAssignmentFileModel>> SaveFilesAsync(
            List<UploadFileModel> files,
            StudentAssignment studentAssignment,
            string driveFolderId,
            DateTime completedAt,
            string teacherEmail)
        {
            List<StudentAssignmentFile> studentAssignmentFiles = new List<StudentAssignmentFile>();
            var uploadedFilesDetails = new Dictionary<string, DriveFileDetailsModel>();

            foreach (var file in files)
            {
                try
                {
                    var googleDriveFileDetails = await fileService.UploadFileAsync(new UploadDriveFileModel
                    {
                        FileContent = file.FileContent,
                        Name = file.Name,
                        OwnerEmail = studentAssignment.Student.Email,
                        ParentFolderId = driveFolderId
                    });

                    uploadedFilesDetails.Add(googleDriveFileDetails.FileDriveId, googleDriveFileDetails);
                    studentAssignmentFiles.Add(new StudentAssignmentFile
                    {
                        DriveFileId = googleDriveFileDetails.FileDriveId,
                        FileName = file.Name,
                        StudentAssignmentId = studentAssignment.Id
                    });
                }
                catch (Exception ex)
                {
                    //TODO: log ex
                }
            }

            await drivePermissionService.SetPermissionsAsync(
                studentAssignmentFiles.Select(x => x.DriveFileId).ToList(), 
                new List<string> { teacherEmail }, 
                "reader");

            var savedFiles = await unitOfWork.StudentAssignmentFilesRepository.AddRangeAsync(studentAssignmentFiles);
            studentAssignment.CompletedAt = completedAt;
            await unitOfWork.StudentAssignmentsRepository.UpdateAsync(studentAssignment);
            await unitOfWork.SaveChangesAsync();
            return MapToAssignmentFileDetailsModels(savedFiles, uploadedFilesDetails);
        }

        private List<StudentAssignmentFileModel> MapToAssignmentFileDetailsModels(List<StudentAssignmentFile> resources, IDictionary<string, DriveFileDetailsModel> driveFileDetails)
        {
            var resourcesModels = new List<StudentAssignmentFileModel>();
            foreach (var resource in resources)
            {
                if (driveFileDetails.TryGetValue(resource.DriveFileId, out var fileDriveDetails))
                {
                    resourcesModels.Add(new StudentAssignmentFileModel
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

            await unitOfWork.StudentAssignmentFilesRepository.RemoveAsync(studentAssignmentFileId);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
