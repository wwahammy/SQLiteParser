using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outercurve.SQLiteCreateTree
{
    static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> enumerable)
        {
            if (enumerable == null)
                return true;

            return !enumerable.Any();
        }
    }
}
