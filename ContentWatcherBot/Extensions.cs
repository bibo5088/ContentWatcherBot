using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot
{
    public static class Extensions
    {
        public static T SingleOrCreate<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T> creator)
            where T : class
        {
            return source.SingleOrDefault(predicate) ?? creator();
        }

        public static async Task<T> SingleOrCreateAsync<T>(this IEnumerable<T> source, Func<T, bool> predicate,
            Func<Task<T>> creator)
            where T : class
        {
            return source.SingleOrDefault(predicate) ?? await creator();
        }

        public static Uri ToHttp(this Uri uri)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttp,
                Port = -1 //Default port
            };

            return uriBuilder.Uri;
        }

        /// <see cref="http://stackoverflow.com/a/18987605"/>
        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float) array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}