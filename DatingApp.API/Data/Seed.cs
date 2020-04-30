using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userData = File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // foreach (var user in users)
                // {
                //     byte[] passwordHash, passwordSalt;
                //     EncryptPassword("password", out passwordHash, out passwordSalt);

                //     // user.PasswordHash = passwordHash;
                //     // user.PasswordSalt = passwordSalt;
                //     user.UserName = user.UserName.ToLower();

                //     context.Users.Add(user);
                // }

                // context.SaveChanges();

                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "VIP" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "Member" }
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users)
                {
                    user.Photos.SingleOrDefault().IsApproved = true;
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Member").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin",
                };

                var result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

                }
            }
        }

        private static void EncryptPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}