using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(byte[] imageContent, string destinationDirectoryName);
    }
}
