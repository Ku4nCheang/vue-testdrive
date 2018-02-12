using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using netcore.Core.ErrorDescribers;

namespace netcore.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Error> TransformIdentityErrors(this IEnumerable<IdentityError> errors)
        {
            return errors.Select(err => new Error {
                Code = err.Code,
                Description = err.Description
            }).AsEnumerable();
        }
    }
}