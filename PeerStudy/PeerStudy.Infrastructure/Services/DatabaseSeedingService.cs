using PeerStudy.Core.DomainEntities;
using PeerStudy.Infrastructure.AppDbContext;
using PeerStudy.Infrastructure.Helpers;
using PeerStudy.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PeerStudy.Infrastructure.Services
{
    public class DatabaseSeedingService : IDatabaseSeedingService
    {
        private readonly ApplicationDbContext context;
        public DatabaseSeedingService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void InsertTeachers()
        {
            try
            {
                string filePath = @"C:\Users\Aura.LAPTOP-GLQOS0K8\Desktop\disertatie\Project\PeerStudy\PeerStudy\PeerStudy.Infrastructure\teachers.json";

                StreamReader sr = new StreamReader(filePath);
                var jsonData = sr.ReadToEnd();
                var users = JsonSerializer.Deserialize<List<Teacher>>(jsonData);

                if (users == null || !users.Any())
                {
                    return;
                }

                var usersToBeInserted = new List<Teacher>();

                foreach (var user in users)
                {
                    bool userExists = context.Users.Where(x => x.Email == user.Email).Any();
                    if (!userExists)
                    {
                        usersToBeInserted.Add(UpdateUserToInsert(user));
                    }
                }

                context.Users.AddRange(usersToBeInserted);
                context.SaveChanges();
            }
            catch (Exception) { }
        }

        private static Teacher UpdateUserToInsert(Teacher user)
        {
            PasswordHelper.CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = Core.Enums.Role.Teacher;

            return user;
        }
    }
}
