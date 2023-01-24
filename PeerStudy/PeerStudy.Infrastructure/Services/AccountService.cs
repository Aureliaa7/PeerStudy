using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Infrastructure.Interfaces;
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

        public async Task<User> RegisterAsync(RegisterModel registerModel)
        {
            bool userExists = await unitOfWork.UsersRepository.ExistsAsync(u => u.Email.Equals(registerModel.Email));
            if (userExists)
            {
                throw new DuplicateEntityException("A user with the same email already exists!");
            }

            PasswordHelper.CreatePasswordHash(registerModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Email = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Role = registerModel.Role
            };
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            if (registerModel.ProfilePhotoContent != null)
            {
                user.ProfilePhotoName = await imageService.SaveImageAsync(registerModel.ProfilePhotoContent, Constants.ProfileImagesDirector);
            }

            var newUser = await unitOfWork.UsersRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            newUser.PasswordHash = null;
            newUser.PasswordSalt = null;

            return newUser;
        }
    }
}
