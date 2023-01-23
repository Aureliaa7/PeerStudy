using PeerStudy.Core.Interfaces.DomainServices;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public async Task<string> SaveImageAsync(byte[] imageContent, string destinationDirectoryName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
           // currentDirectory = currentDirectory.Replace(Constants.APIAssembly, Constants.ClientAssembly);
            string profilePhtotosDirectory = $"{currentDirectory}\\{destinationDirectoryName}";

            if (!Directory.Exists(profilePhtotosDirectory))
            {
                Directory.CreateDirectory(profilePhtotosDirectory);
            }

            // let it be a simple GUID since the image name is not important
            string fileName = Guid.NewGuid().ToString() + ".png";
            string filePath = Path.Combine(profilePhtotosDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, imageContent);

            // Return the fileName and not its path because otherwise I cannot access the image from Client
            return fileName;
        }
    }
}
