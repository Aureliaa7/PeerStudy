using PeerStudy.Core.Models.Assignments;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAssignmentFileService
    {
        Task<AssignmentFilesModel> GetUploadedFilesByStudentAsync(Guid assignmentId, Guid studentId);

        Task<List<StudentAssignmentFileModel>> UploadWorkAsync(UploadAssignmentFilesModel model);
    }
}
