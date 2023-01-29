using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, string.Concat(user.FirstName, " ", user.LastName)),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(Constants.UserId, user.Id.ToString())
            };

            string jwtKey = configuration.GetSection("JWTKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(90),
                signingCredentials: credentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
    }
}
