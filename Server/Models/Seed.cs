using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using netcore.Core.Extensions;
using netcore.Models.Contexts;

namespace netcore.Models
{
    public class Seed 
    {
        public Seed(
            ApplicationContext context,
            UserManager<User> userManager,
            RoleManager<UserRole> roleManager
        )
        {
            Context = context;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        protected readonly ApplicationContext Context;
        protected readonly UserManager<User> UserManager;
        protected readonly RoleManager<UserRole> RoleManager;
        public string ResourcePath = "";


        //
        // ─── SEEDING REQUIRED DATA ───────────────────────────────────────
        //

        public async Task AddFixture()
        {
            await _EnsureUserData();
        }

        private async Task _EnsureUserData()
        {
            if (!Context.UserRoles.Any())
            {
                await RoleManager.CreateAsync(new UserRole("Member"));
                await RoleManager.CreateAsync(new UserRole("SystemUser"));
                await RoleManager.CreateAsync(new UserRole("Administrator"));
            }

            if (!Context.Users.Any())
            {
                var u1 = new User() {
                    Id = "24b764b9-e9f4-4a51-999b-908e60fad6df",
                    UserName = "1123456789123",
                    Email = "user1@example.com",
                    DisplayName = "Test User A",
                    FirstName = "User",
                    LastName = "A",
                    JoinedAt = DateTimeOffset.UtcNow.DateTime
                };
                await UserManager.CreateAsync(u1, "P@ssword");
                await UserManager.AddToRoleAsync(u1, "Member");

                var u2 = new User() {
                    Id = "e1c8a09c-1417-4d84-8223-86ab3994168c",
                    UserName = "1123456789124",
                    Email = "user2@example.com",
                    DisplayName = "Test User B",
                    FirstName = "User",
                    LastName = "B",
                    JoinedAt = DateTimeOffset.UtcNow.DateTime
                };
                await UserManager.CreateAsync(u2, "P@ssword");
                await UserManager.AddToRoleAsync(u2, "SystemUser");
                await UserManager.AddToRoleAsync(u2, "Member");

                var u3 = new User() {
                    Id = "6be3d01e-8884-4aea-bc66-bc252a6c25f5",
                    UserName = "1123456789125",
                    Email = "user3@example.com",
                    DisplayName = "Test User C",
                    FirstName = "User",
                    LastName = "C",
                    JoinedAt = DateTimeOffset.UtcNow.DateTime
                };
                await UserManager.CreateAsync(u3, "P@ssword");
                await UserManager.AddToRoleAsync(u3, "Member");
                await UserManager.AddToRoleAsync(u3, "SystemUser");
                await UserManager.AddToRoleAsync(u3, "Administrator");
            }
        }

        //
        // ─── PRIVATE METHODS ─────────────────────────────────────────────
        //

        private string _GetStringFromFile(string fileName)
        {
            var filePath = Path.Combine(ResourcePath, fileName);
            var jsonString = System.IO.File.ReadAllText(filePath);
            return jsonString;
        }
        
        private T _GetObjectFromJsonFile<T>(string fileName)
        {
            var jsonString = this._GetStringFromFile(fileName);
            return jsonString.ToJson<T>();
        }
    }
}