using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using netcore.Core.Configurations;
using netcore.Core.Constants;
using netcore.Core.ErrorDescribers;
using netcore.Core.Extensions;
using netcore.Core.Services;
using netcore.Models;

namespace netcore.Controllers.API
{
    public class ApiController<T> : LoggerController<T> where T : class
    {
        /// <summary>
        /// A automapper allow to map specified properties of an object into that of other object 
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// A server in-memory cache for quickly access data in the memory
        /// </summary>
        protected IMemoryCache MemoryCache { get; }

        /// <summary>
        /// User manager for managing user
        /// </summary>
        protected UserManager<User> UserManager { get; }

        /// <summary>
        /// Accessor for the application settings
        /// </summary>
        protected AppSettings AppSettings { get; }

        public ApiController(IServiceProvider serviceProvider): base(serviceProvider) {
            // Get service from static service provider so
            // we don't need each subclass has to inject following services.
            MemoryCache = serviceProvider.GetService(typeof(IMemoryCache)) as IMemoryCache;
            Mapper = serviceProvider.GetService(typeof(IMapper)) as IMapper;
            UserManager = serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>;
            AppSettings = serviceProvider.GetService(typeof(AppSettings)) as AppSettings;
        }

        /// <summary>
        /// Get memory cache value from in-memory store in current application server
        /// </summary>
        /// <param name="key">Cache key for the entry</param>
        /// <returns>Cached value with specified type, return null if empty</returns>
        public R GetCacheEntry<R> (string key) where R: class {
            object entry = null;
            if (MemoryCache.TryGetValue(key, out entry)) {
                return entry as R;
            }
            return null;
        }

        /// <summary>
        /// Get current logon user
        /// </summary>
        /// <returns>User who has logon</returns>
        protected async Task<User> GetCurrentUserAsync()
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            return await UserManager.GetUserAsync(User);
        }
    }
}