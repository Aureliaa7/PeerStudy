﻿using PeerStudy.Core.DomainEntities;
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

        public AssignmentFileService(IUnitOfWork unitOfWork, IGoogleDriveFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
        }


        //TODO: when uploading files, the teacher should have read permissions, and the student write permissions
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
                CompletedAt = studentAssignment.CompletedAt
            };

            assignmentFilesModel.StudentAssignmentFiles = (await unitOfWork.StudentAssignmentFilesRepository.GetAllAsync(x =>
                x.StudentAssignment.AssignmentId == assignmentId && x.StudentAssignment.StudentId == studentId))
            .Select(x => new StudentAssignmentFileModel
            {
                FileDriveId = x.DriveFileId,
                Name = x.FileName
            })
            .ToList();

            return assignmentFilesModel;
        }

        public async Task<List<StudentAssignmentFileModel>> UploadWorkAsync(UploadAssignmentFilesModel model)
        {
            // get student email and teacher email, also add app email
            // get assignment resource folder id
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

            List<StudentAssignmentFileModel> uploadedFiles = new List<StudentAssignmentFileModel>();

            return await SaveFilesAsync(model.Files, studentAssignment, assignmentDetails.AssignmentsDriveFolderId);
        }

        private async Task<List<StudentAssignmentFileModel>> SaveFilesAsync(
            List<UploadFileModel> files,
            StudentAssignment studentAssignment,
            string driveFolderId)
        {
            List<StudentAssignmentFile> studentAssignmentFiles = new List<StudentAssignmentFile>();
            var uploadedFilesDetails = new Dictionary<string, FileDetailsModel>();

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

                    //TODO: give teacher read permission; 
                }
                catch (Exception ex)
                {
                    //TODO: log ex
                }
            }

            var savedFiles = await unitOfWork.StudentAssignmentFilesRepository.AddRangeAsync(studentAssignmentFiles);
            studentAssignment.CompletedAt = DateTime.UtcNow;
            await unitOfWork.StudentAssignmentsRepository.UpdateAsync(studentAssignment);
            await unitOfWork.SaveChangesAsync();
            return MapToAssignmentFileDetailsModels(savedFiles, uploadedFilesDetails);
        }

        private List<StudentAssignmentFileModel> MapToAssignmentFileDetailsModels(List<StudentAssignmentFile> resources, IDictionary<string, FileDetailsModel> driveFileDetails)
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
    }
}