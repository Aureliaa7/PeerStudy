using System.IO;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IGoogleDriveFileService
    {
        Task<string> CreateFolderAsync(string folderName, string parent = null);

        Task<string> UploadFileAsync(string parentId, Stream file, string fileName);
    }
}
