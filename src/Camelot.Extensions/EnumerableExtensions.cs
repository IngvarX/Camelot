using System;
using System.Collections.Generic;

namespace Camelot.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}