﻿using System.Linq;
using Esquire.Models;
 
using static Esquire.Data.SeedAvatars;

namespace Esquire.Data
{
    public class DbInitializer
    {
        public static void Initialize(ProjectContext context)
        {            
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new []
                {
                    CreateUser("tester1@test.gov", "Tester", "1", "Admin", tester1),
                    CreateUser("tester2@test.gov", "Tester", "2", "Admin", tester2),
                    CreateUser("tester3@test.gov", "Tester", "3", "Admin", tester3),
                    CreateUser("tester4@test.gov", "Tester", "4", "Admin", tester4)
                };

                foreach (var u in users)
                {
                    context.Users.Add(u);
                }
            }
            
            context.SaveChanges();
        }

        private static User CreateUser(string email, string firstName, string lastName, string role, string avatar = "")
        {
            return new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role, //"Admin", "Coder", "Coordinator"
                IsActive = true,
                Avatar = avatar
            };
        }

    }
}
