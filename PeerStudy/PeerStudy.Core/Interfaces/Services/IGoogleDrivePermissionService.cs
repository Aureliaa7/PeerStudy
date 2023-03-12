using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.Services
{
    public interface IGoogleDrivePermissionService
    {
        Task SetPermissionsAsync(List<string> fileIds, List<string> emails, string role);
    }
}
