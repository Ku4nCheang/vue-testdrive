using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Core.Contants;
using netcore.Models;
using netcore.Models.Contexts;

namespace netcore.Core.Repositories
{
    public class UserRepository : Repository<ApplicationContext>, IUserRepository
    {
        public UserRepository(IServiceProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Retrieved all the user from persistent store.
        /// </summary>
        /// <returns>List of users that in store.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            ThrowIfDisposed();

            try
            {
                var users = await Context.Users
                    .AsNoTracking()
                    .ToListAsync(CancellationToken);

                Logger.LogInformation(EventIds.GetAllUsers, $"Retrieved users ({users.Count}) from persistent store.");

                return users;
            }
            catch (Exception ex)
            {
                Logger.LogError(EventIds.GetAllUsersError, ex, "Could not retrieve all users from persistent store.");
            }

            return null;
        }

        /// <summary>
        /// Retrieved users at specified page from persistent store.
        /// </summary>
        /// <param name="count">number of users that will be retrieved.</param>
        /// <param name="page">which page of users that will be retrieved.</param>
        /// <returns>List of users that in store.</returns>
        public async Task<List<User>> GetUsersAsync(int count = 30, int page = 1)
        {
            ThrowIfDisposed();

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var skip = (page - 1) * count;

            try
            {
                var users = await Context.Users
                    .OrderBy(u => u.UserName)
                    .Skip(skip)
                    .Take(count)
                    .AsNoTracking()
                    .ToListAsync(CancellationToken);

                Logger.LogInformation(EventIds.GetUsersAtPage, $"Retrieved users ({users.Count}) for page ({page}) from persistent store.");

                return users;
            }
            catch (Exception ex)
            {
                Logger.LogError(EventIds.GetUsersAtPageError, ex, $"Could not retrieve users for page ({page}) from persistent store.");
            }

            return null;
        }

        public Task<int> CountUsersAsync()
        {
            return Context.Users.CountAsync();
        }
    }
}