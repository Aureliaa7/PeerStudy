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
    public class StudentPointsService : IStudentPointsService
    {
        private readonly IUnitOfWork unitOfWork;

        public StudentPointsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AddAsync(SaveStudentPointsModel saveStudentPointsModel)
        {
            var student = await GetStudentAsync(saveStudentPointsModel.StudentId);

            student.NoTotalPoints += saveStudentPointsModel.NoPoints;
            await unitOfWork.UsersRepository.UpdateAsync(student);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetNoPointsAsync(Guid studentId)
        {
            var student = await GetStudentAsync(studentId);

            return student.NoTotalPoints;
        }

        public async Task SubtractPointsAsync(Guid studentId, int noPoints, bool saveChanges = true)
        {
            var student = await GetStudentAsync(studentId);

            if (student.NoTotalPoints < noPoints)
            {
                throw new PreconditionFailedException($"Student with id {studentId} does not have enough points!");
            }

            student.NoTotalPoints -= noPoints;
            await unitOfWork.UsersRepository.UpdateAsync(student);

            if (saveChanges)
            {
                await unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<Student> GetStudentAsync(Guid id)
        {
            var student = (Student)await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == id
                && x.Role == Role.Student) ?? throw new EntityNotFoundException($"Student with id {id} was not found!");

            return student;
        }
    }
}
