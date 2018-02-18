using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace netcore.Core.Repositories
{
    public class Repository<C>: IDisposable where C : DbContext
    {

        public Repository(IServiceProvider provider)
        {
            var loggerFactory = provider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            Logger = loggerFactory.CreateLogger(this.GetType());
            Context = provider.GetService(typeof(C)) as C;
        }

        /// <summary>
        /// Logger for repository
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Context for repository
        /// </summary>
        protected readonly C Context;

        /// <summary>
        /// Default cancellation token for this repository instance
        /// </summary>
        /// <returns>Cancellation token</returns>
        protected CancellationToken CancellationToken => default(CancellationToken);

        /// <summary>
        /// A flag to indicate whether the store is disposed or not
        /// </summary>
        private bool _Disposed { get; set; }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_Disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose the stores
        /// </summary>
        public void Dispose()
        {
            _Disposed = true;
        }
    }
}