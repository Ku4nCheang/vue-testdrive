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
                    .Where(u => !u.Deleted)
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

        public async Task<List<User>> GetUsersAsync(int count = 30, int page = 1, UserOrderBy orderBy = UserOrderBy.UserName, UserOrderDir dir = UserOrderDir.Ascending)
        {
            ThrowIfDisposed();

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var skip = (page - 1) * count;
            var asc = Convert.ToBoolean((int)dir);

            var sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.UserName ) :
                    Context.Users.OrderByDescending( u => u.UserName );

            if (orderBy == UserOrderBy.DateOfBirth)
            {
                sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.DateOfBirth ) :
                    Context.Users.OrderByDescending( u => u.DateOfBirth );
            }
            else if (orderBy == UserOrderBy.Email)
            {
                sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.Email ) :
                    Context.Users.OrderByDescending( u => u.Email );
            }
            else if (orderBy == UserOrderBy.Gender)
            {
                sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.Gender ) :
                    Context.Users.OrderByDescending( u => u.Gender );
            }
            else if (orderBy == UserOrderBy.JoinedAt)
            {
                sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.JoinedAt ) :
                    Context.Users.OrderByDescending( u => u.JoinedAt );
            }
            else if (orderBy == UserOrderBy.LastName)
            {
                sortedUsers = (asc) ? 
                    Context.Users.OrderBy( u => u.LastName ) :
                    Context.Users.OrderByDescending( u => u.LastName );
            }

            try
            {
                var users = await sortedUsers
                    .Where(u => !u.Deleted)
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
            return Context.Users.CountAsync( user => !user.Deleted );
        }
    }
}