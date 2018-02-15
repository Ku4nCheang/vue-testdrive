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
                    UserName = "1123456789123413",
                    Email = "user1@example.com",
                    DisplayName = "Test User A",
                    JoinedAt = DateTimeOffset.UtcNow.DateTime
                };
                await UserManager.CreateAsync(u1, "P@ssword");
                await UserManager.AddToRoleAsync(u1, "Member");

                var u2 = new User() {
                    UserName = "1123456789123415",
                    Email = "user2@example.com",
                    DisplayName = "Test User B",
                    JoinedAt = DateTimeOffset.UtcNow.DateTime
                };
                await UserManager.CreateAsync(u2, "P@ssword");
                await UserManager.AddToRoleAsync(u2, "SystemUser");
                await UserManager.AddToRoleAsync(u2, "Member");

                var u3 = new User() {
                    UserName = "1123456789123419",
                    Email = "user3@example.com",
                    DisplayName = "Test User C",
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