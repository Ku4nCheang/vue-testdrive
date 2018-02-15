using System.Collections.Generic;
using System.Threading.Tasks;
using netcore.Models;

namespace netcore.Core.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieved all the user from persistent store.
        /// </summary>
        /// <returns>List of users that in store.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Retrieved users at specified page from persistent store.
        /// </summary>
        /// <param name="count">number of users that will be retrieved.</param>
        /// <param name="page">which page of users that will be retrieved.</param>
        /// <returns>List of users that in store.</returns>
        Task<List<User>> GetUsersAsync(int count = 30, int page = 1);


        /// <summary>
        /// Get the total number of users in persistent store.
        /// </summary>
        /// <returns>Total number of users</returns>
        Task<int> CountUsersAsync();
    }
}