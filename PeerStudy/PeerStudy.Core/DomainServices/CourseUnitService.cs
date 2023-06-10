using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class CourseUnitService : ICourseUnitService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICourseResourceService courseResourceService;
        private readonly IStudentPointsService studentPointsService;

        public CourseUnitService(IUnitOfWork unitOfWork,
            ICourseResourceService courseResourceService,
            IStudentPointsService studentPointsService)
        {
            this.unitOfWork = unitOfWork;
            this.courseResourceService = courseResourceService;
            this.studentPointsService = studentPointsService;
        }

        public async Task<CourseUnitDetailsModel> CreateAsync(CourseUnitModel courseUnit)
        {
            bool courseExists = await unitOfWork.CoursesRepository.ExistsAsync(x => x.Id == courseUnit.CourseId);

            if (!courseExists)
            {
                throw new EntityNotFoundException($"Course with id {courseUnit.CourseId} was not found!");
            }

            var savedCourseUnit = await unitOfWork.CourseUnitsRepository.AddAsync(new CourseUnit
            {
                CourseId = courseUnit.CourseId,
                Title = courseUnit.Title,
                IsAvailable = courseUnit.IsAvailable,
                NoPointsToUnlock = courseUnit.NoPointsToUnlock
            });
            await unitOfWork.SaveChangesAsync();

            return new CourseUnitDetailsModel
            {
                Id = savedCourseUnit.Id,
                Title = courseUnit.Title,
                CourseId = courseUnit.CourseId,
                Resources = new List<CourseResourceDetailsModel>(),
                IsAvailable = courseUnit.IsAvailable,
                NoPointsToUnlock = courseUnit.NoPointsToUnlock,
                IsActive = true
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            await CheckIfCourseUnitExistsAsync(id);

            var courseResourceIds = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => x.CourseUnitId == id))
                .Select(x => x.Id)
                .ToList();

            await courseResourceService.DeleteRangeAsync(courseResourceIds);

            await unitOfWork.CourseUnitsRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CourseUnitDetailsModel>> GetByCourseAndStudentIdAsync(Guid courseId, Guid studentId)
        {
            var courseUnits = await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.CourseId == courseId);

            var unlockedCourseUnits = await unitOfWork.UnlockedCourseUnitsRepository.GetAllAsync(x => x.StudentId == studentId &&
            x.CourseUnit.CourseId == courseId);
            var noResources = new List<CourseResourceDetailsModel>();

            var result = from courseUnit in courseUnits
                         join unlockedUnit in unlockedCourseUnits
                         on courseUnit.Id equals unlockedUnit.CourseUnitId
                         into foundResult
                         from foundUnlockedCourseUnit in foundResult.DefaultIfEmpty()
                         select new CourseUnitDetailsModel
                         {
                             IsActive = courseUnit.Course.Status == CourseStatus.Active,
                             IsAvailable = courseUnit.IsAvailable || foundUnlockedCourseUnit != null,
                             CourseId = courseId,
                             Id = courseUnit.Id,
                             NoPointsToUnlock = courseUnit.NoPointsToUnlock,
                             Title = courseUnit.Title,
                             Resources = noResources,
                             Order = courseUnit.Order
                         };

            var foundCourseUnits = await SetResourcesAsync(courseId, result.ToList());

            return foundCourseUnits;
        }

        public async Task<List<CourseUnitDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            var noResources = new List<CourseResourceDetailsModel>();

            var courseUnits =
                (await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.CourseId == courseId))
                .Select(x => new CourseUnitDetailsModel
                {
                    IsActive = x.Course.Status == CourseStatus.Active,
                    CourseId = x.CourseId,
                    Id = x.Id,
                    Title = x.Title,
                    IsAvailable = x.IsAvailable,
                    NoPointsToUnlock = x.NoPointsToUnlock,
                    Resources = noResources
                })
                .ToList();
            var result = await SetResourcesAsync(courseId, courseUnits);

            return result;
        }

        private async Task<List<CourseUnitDetailsModel>> SetResourcesAsync(Guid courseId, List<CourseUnitDetailsModel> courseUnits)
        {
            var resources = await courseResourceService.GetByCourseIdAsync(courseId);
            var resourcesGroupedByCourseUnit = resources
                .GroupBy(x => x.CouseUnitId)
                .ToList();

            var courseUnitIdDetailsPairs = courseUnits.ToDictionary(x => x.Id, x => x);
            var noResources = new List<CourseResourceDetailsModel>();

            foreach (var group in resourcesGroupedByCourseUnit)
            {
                if (courseUnitIdDetailsPairs.TryGetValue(group.Key, out var courseUnit))
                {
                    courseUnit.Resources = group
                        .Select(x => x)
                        .ToList();
                }
            }

            return courseUnits;
        }

        public async Task RenameAsync(Guid id, string newName)
        {
            var courseUnit = await unitOfWork.CourseUnitsRepository.GetByIdAsync(id) ?? 
                throw new EntityNotFoundException($"Course unit with id {id} was not found!");
            
            courseUnit.Title = newName;
            await unitOfWork.CourseUnitsRepository.UpdateAsync(courseUnit);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UnlockAsync(Guid courseUnitId, Guid studentId)
        {
            var courseUnitIsUnlocked = await unitOfWork.UnlockedCourseUnitsRepository
                .ExistsAsync(x => x.CourseUnitId == courseUnitId && x.StudentId == studentId);
            if (courseUnitIsUnlocked)
            {
                throw new DuplicateEntityException($"Course unit with id {courseUnitId} is already unlocked for student with id {studentId}");
            }

            var courseUnit = await unitOfWork.CourseUnitsRepository.GetFirstOrDefaultAsync(x => x.Id == courseUnitId) ?? 
                throw new EntityNotFoundException($"Course unit with id {courseUnitId} was not found!");

            if (courseUnit.Order > 1 &&
                await CheckPreviousCourseUnitsAvailabilityAsync(courseUnit.Order, studentId))
            {
                await studentPointsService.SubtractPointsAsync(studentId, courseUnit.NoPointsToUnlock, false);
                await unitOfWork.UnlockedCourseUnitsRepository.AddAsync(new UnlockedCourseUnit
                {
                    CourseUnitId = courseUnitId,
                    StudentId = studentId,
                    UnlockedAt = DateTime.UtcNow
                });

                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckPreviousCourseUnitsAvailabilityAsync(int courseUnitOrder, Guid studentId)
        {
            if (courseUnitOrder > 1)
            {
                var previousLockedCourseUnitsIds = (await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.Order < courseUnitOrder && !x.IsAvailable))
                    .Select(x => x.Id)
                    .ToList();

                var noUnlockedCourseUnits = await unitOfWork.UnlockedCourseUnitsRepository.GetTotalRecordsAsync(x =>
                    previousLockedCourseUnitsIds.Contains(x.CourseUnitId) && x.StudentId == studentId);

                if (previousLockedCourseUnitsIds.Count > noUnlockedCourseUnits)
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException("Course unit order should be greater than 1!");
            }

            return true;
        }

        private async Task CheckIfCourseUnitExistsAsync(Guid id)
        {
            bool courseUnitExists = await unitOfWork.CourseUnitsRepository.ExistsAsync(x => x.Id == id);

            if (!courseUnitExists)
            {
                throw new EntityNotFoundException($"Course unit with id {id} was not found!");
            }
        }
    }
}
