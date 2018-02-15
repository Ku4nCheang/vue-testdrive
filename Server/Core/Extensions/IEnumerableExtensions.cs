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
        /// <summary>
        /// Transform the identity errors into custom type errors.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static IEnumerable<Error> TransformIdentityErrors(this IEnumerable<IdentityError> errors)
        {
            return errors.Select(err => new Error {
                Code = err.Code,
                Description = err.Description
            }).AsEnumerable();
        }

        /// <summary>
        /// Loop through each item in the list.
        /// </summary>
        /// <param name="items">Items for loop</param>
        /// <param name="action">Perform an action on the item</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach(var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Mapping the items into another items
        /// </summary>
        /// <param name="items">Items will be mapped</param>
        /// <param name="mapAction">Action will used to map existing item into new item</param>
        /// <returns>New items</returns>
        public static IEnumerable<T> Map<T>(this IEnumerable<string> items, Func<string, T> mapAction)
        {
            var newItems = new List<T>();
            foreach (var item in items)
            {
                newItems.Add(mapAction(item));
            }
            return newItems.AsEnumerable();
        }
    }
}