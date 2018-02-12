using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netcore.Core.ErrorDescribers;

namespace netcore.Controllers
{
    public class LoggerController<T> : Controller where T : class
    {
        /// <summary>
        /// Constructor for Logger Controller
        /// </summary>
        public LoggerController(IServiceProvider serviceProvider): base() {
            // Get service from static service provider so
            // we don't need each subclass has to inject following services.
            this.Logger = (serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory).CreateLogger<T>();
        }

        /// <summary>
        /// Logger for the controller
        /// </summary>
        protected ILogger<T> Logger { get; set; }

        /// <summary>
        /// Get all the error codes for logging.
        /// </summary>
        /// <param name="errors">Error object contains code and description</param>
        /// <returns>A joined error codes string</returns>
        public string GetErrorCodes(IEnumerable<Error> errors) {
            var codes = errors.Select(err => err.Code).ToList();
            return String.Join(",", codes);
        }
    }
}