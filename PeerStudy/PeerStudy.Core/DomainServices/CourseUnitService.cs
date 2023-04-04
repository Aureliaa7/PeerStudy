using PeerStudy.Core.DomainEntities;
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

        public CourseUnitService(IUnitOfWork unitOfWork, ICourseResourceService courseResourceService)
        {
            this.unitOfWork = unitOfWork;
            this.courseResourceService = courseResourceService;
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
                Title = courseUnit.Title
            });
            await unitOfWork.SaveChangesAsync();

            return new CourseUnitDetailsModel
            {
                Id = savedCourseUnit.Id,
                Title = courseUnit.Title,
                CourseId = courseUnit.CourseId,
                Resources = new List<CourseResourceDetailsModel>()
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            bool courseUnitExists = await unitOfWork.CourseUnitsRepository.ExistsAsync(x => x.Id == id);

            if (!courseUnitExists)
            {
                throw new EntityNotFoundException($"Course unit with id {id} was not found!");
            }

            var courseResourceIds = (await unitOfWork.CourseResourcesRepository.GetAllAsync(x => x.CourseUnitId == id))
                .Select(x => x.Id)
                .ToList();

            await courseResourceService.DeleteRangeAsync(courseResourceIds);

            await unitOfWork.CourseUnitsRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CourseUnitDetailsModel>> GetByCourseIdAsync(Guid courseId)
        {
            var resources = await courseResourceService.GetByCourseIdAsync(courseId);
            var resourcesGroupedByCourseUnit = resources
                .GroupBy(x => x.CouseUnitId)
                .ToList();

            var noResources = new List<CourseResourceDetailsModel>();

            var courseUnits =
                (await unitOfWork.CourseUnitsRepository.GetAllAsync(x => x.CourseId == courseId))
                .Select(x => new CourseUnitDetailsModel
                {
                    IsActive = x.Course.Status == Enums.CourseStatus.Active,
                    CourseId = x.CourseId,
                    Id = x.Id,
                    Title = x.Title,
                    Resources = noResources
                })
                .ToList();

            var courseUnitIdDetailsPairs = courseUnits.ToDictionary(x => x.Id, x => x);

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
    }
}
