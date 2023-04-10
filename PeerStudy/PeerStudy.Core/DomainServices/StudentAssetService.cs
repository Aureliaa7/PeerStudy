using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.StudentAssets;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class StudentAssetService : IStudentAssetService
    {
        private readonly IUnitOfWork unitOfWork;

        public StudentAssetService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateStudentAssetModel createAssetModel)
        {
            var studentAssetExists = await unitOfWork.StudentAssetsRepository.ExistsAsync(x => x.Asset == createAssetModel.AssetType &&
                x.StudentId == createAssetModel.StudentId);

            if (studentAssetExists)
            {
                throw new DuplicateEntityException($"An entity with asset type {createAssetModel.AssetType.ToString()} and studentId {createAssetModel.StudentId} already exists!");
            }

            await unitOfWork.StudentAssetsRepository.AddAsync(new StudentAsset
            {
                Asset = createAssetModel.AssetType,
                StudentId = createAssetModel.StudentId,
                NumberOfAssets = createAssetModel.NoAssets
            });
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<StudentAsset> GetAsync(AssetType assetType, Guid studentId)
        {
            var asset = await unitOfWork.StudentAssetsRepository.GetFirstOrDefaultAsync(
              x => x.Asset == assetType && x.StudentId == studentId);

            return asset;
        }

        public async Task<int> GetNoAssetsAsync(AssetType assetType, Guid studentId)
        {
            var asset = await GetAsync(assetType, studentId);

            return asset == null ? 0 : asset.NumberOfAssets;
        }
    }
}
