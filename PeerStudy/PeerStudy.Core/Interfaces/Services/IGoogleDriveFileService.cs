using PeerStudy.Core.Models.Resources;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IGoogleDriveFileService
    {
        Task<string> CreateFolderAsync(string folderName, string parent = null);

        Task<string> UploadFileAsync(UploadFileModel model);
    }
}
