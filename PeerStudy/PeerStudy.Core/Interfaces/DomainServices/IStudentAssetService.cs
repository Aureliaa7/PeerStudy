using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.StudentAssets;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IStudentAssetService
    {
        Task<int> GetNoAssetsAsync(AssetType assetType, Guid studentId);

        Task<StudentAsset> GetAsync(AssetType assetType, Guid studentId);

        Task CreateAsync(CreateStudentAssetModel createAssetModel);
    }
}
