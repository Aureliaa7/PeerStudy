using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Accounts;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IJwtService jwtService;
        private readonly IImageService imageService;

        public AccountService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IImageService imageService)
        {
            this.unitOfWork = unitOfWork;
            this.jwtService = jwtService;
            this.imageService = imageService;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            User user = await unitOfWork.UsersRepository.GetByIdAsync(changePasswordModel.UserId);
            if (user == null)
            {
                return false;
            }

            bool isOldPasswordCorrect = PasswordHelper.IsCorrectPasswordHash(
                changePasswordModel.OldPassword, 
                user.PasswordHash, 
                user.PasswordSalt);

            if (isOldPasswordCorrect)
            {
                PasswordHelper.CreatePasswordHash(
                    changePasswordModel.NewPassword, 
                    out byte[] newPasswordHash, 
                    out byte[] newPasswordSalt);
                user.PasswordHash = newPasswordHash;
                user.PasswordSalt = newPasswordSalt;
                await unitOfWork.UsersRepository.UpdateAsync(user);
                await unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<UserDetailsModel> GetUserDetailsAsync(Guid userId)
        {
            var user = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(
                x => x.Id == userId) ?? throw new EntityNotFoundException($"User with id {userId} was not found!");

            return new UserDetailsModel
            {
                Id = userId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePhotoName = user.ProfilePhotoName,
            };
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            string token = string.Empty;
            try
            {
                token = await jwtService.GenerateTokenAsync(loginModel);
            }
            catch (EntityNotFoundException)
            {
            }

            return token;
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            bool userExists = await unitOfWork.UsersRepository.ExistsAsync(u => u.Email.Equals(registerModel.Email));
            if (userExists)
            {
                throw new DuplicateEntityException("A user with the same email already exists!");
            }

            PasswordHelper.CreatePasswordHash(registerModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
           
            var userToBeInserted = GetUserToBeInserted(registerModel);

            userToBeInserted.PasswordHash = passwordHash;
            userToBeInserted.PasswordSalt = passwordSalt;
            if (registerModel.ProfilePhotoContent != null)
            {
                userToBeInserted.ProfilePhotoName = await imageService.SaveImageAsync(
                    registerModel.ProfilePhotoContent, 
                    Constants.ProfileImagesDirector);
            }

            var newUser = await unitOfWork.UsersRepository.AddAsync(userToBeInserted);
            await unitOfWork.SaveChangesAsync();
        }

        private User GetUserToBeInserted(RegisterModel registerModel)
        {
            User userToBeInserted = null;

            if (registerModel.Role == Role.Student)
            {
                userToBeInserted = new Student
                {
                    Email = registerModel.Email,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Role = registerModel.Role
                };
            }
            else if (registerModel.Role == Role.Teacher)
            {
                userToBeInserted = new Teacher
                {
                    Email = registerModel.Email,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Role = registerModel.Role
                };
            }
            else
            {
                throw new Exception($"{registerModel.Role} role is not supported by PeerStudy!");
            }

            return userToBeInserted;
        }
    }
}
