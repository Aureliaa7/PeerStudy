using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAssignmentFileService
    {
        Task<AssignmentFilesModel> GetUploadedFilesByStudyGroupAsync(Guid assignmentId, Guid studyGroupId);

        Task<List<StudyGroupAssignmentFileModel>> UploadWorkAsync(UploadAssignmentFilesModel model, DateTime completedAt);

        Task DeleteAsync(string driveFileId, Guid studentAssignmentFileId);
    }
}
