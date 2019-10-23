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
    }
}