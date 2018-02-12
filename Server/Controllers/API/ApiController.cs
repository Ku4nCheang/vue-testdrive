using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using netcore.Core.ErrorDescribers;
using netcore.Core.Extensions;
// using netcore.Core.Repositories;
using netcore.Core.Services;

namespace netcore.Controllers
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

        public ApiController(IServiceProvider serviceProvider): base(serviceProvider) {
            // Get service from static service provider so
            // we don't need each subclass has to inject following services.
            this.MemoryCache = serviceProvider.GetService(typeof(IMemoryCache)) as IMemoryCache;
            this.Mapper = serviceProvider.GetService(typeof(IMapper)) as IMapper;
        }

        /// <summary>
        /// Get memory cache value from in-memory store in current application server
        /// </summary>
        /// <param name="key">Cache key for the entry</param>
        /// <returns>Cached value with specified type, return null if empty</returns>
        public R GetCacheEntry<R> (string key) where R: class {
            object entry = null;
            if (this.MemoryCache.TryGetValue(key, out entry)) {
                return entry as R;
            }
            return null;
        }
    }
}