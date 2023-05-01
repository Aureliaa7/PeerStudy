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
    public class BadgeService : IBadgeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IStudentPointsService studentPointsService;

        public BadgeService(IUnitOfWork unitOfWork, IStudentPointsService studentPointsService)
        {
            this.unitOfWork = unitOfWork;
            this.studentPointsService = studentPointsService;
        }

        public async Task AddAsync(Guid studentId, BadgeType badgeType)
        {
            var studentBadge = await unitOfWork.StudentBadgesRepository.GetFirstOrDefaultAsync(x => x.StudentId == studentId
            && x.Badge.Type == badgeType);
            var badge = await unitOfWork.BadgesRepository.GetFirstOrDefaultAsync(x => x.Type == badgeType) 
                ?? throw new EntityNotFoundException($"Badge {badgeType.ToString()} was not found!");

            if (studentBadge == null)
            {
                await unitOfWork.StudentBadgesRepository.AddAsync(new StudentBadge
                {
                    StudentId = studentId,
                    Badge = badge,
                    NumberOfBadges = 1
                });                
            }
            else
            {
                studentBadge.NumberOfBadges++;
                await unitOfWork.StudentBadgesRepository.UpdateAsync(studentBadge);
            }

            await unitOfWork.SaveChangesAsync();
            await studentPointsService.AddAsync(new SaveStudentPointsModel
            {
                StudentId = studentId,
                NoPoints = badge.Points
            });
        }
    }
}
