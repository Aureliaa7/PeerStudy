using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Infrastructure.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public JwtService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public async Task<string> GenerateTokenAsync(LoginModel loginModel)
        {
            var user = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                throw new EntityNotFoundException("The user was not found!");
            }

            bool isCorrectPassword = PasswordHelper.IsCorrectPasswordHash(loginModel.Password, user.PasswordHash, user.PasswordSalt);

            if (!isCorrectPassword)
            {
                throw new EntityNotFoundException("Wrong credentials!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            string jwtKey = configuration.GetSection("JWTKey").Value;
            var tokenKeyBytes = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, loginModel.Email),
                    new Claim(Constants.UserFullName, string.Concat(user.FirstName, " ", user.LastName)),
                    new Claim(Constants.UserId, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
