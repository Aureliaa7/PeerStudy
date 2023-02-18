using PeerStudy.Core.Models.GoogleDriveModels;
using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IGoogleDriveFileService
    {
        Task<string> CreateFolderAsync(string folderName, string parent = null);

        Task<FileDetailsModel> UploadFileAsync(UploadFileModel model);

        Task<Dictionary<string, FileDetailsModel>> GetFilesDetailsAsync(List<string> fileIds);

        Task DeleteAsync(string resourceId);
    }
}
