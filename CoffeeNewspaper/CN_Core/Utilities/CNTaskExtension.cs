using System.Collections.Generic;
using System.Linq;

namespace CN_Core.Utilities
{
    public static class CNTaskExtension
    {
        /// <summary>
        /// Get Default Selection Filter And Order
        /// </summary>
        /// <returns></returns>
        public static IOrderedEnumerable<CNTask> FilterDeletedAndOrderBySortTasks(this IEnumerable<CNTask> originEnumerable)
        {
            return from c in originEnumerable
                where !c.IsDeleted
                orderby c.Sort
                select c;
        }
    }
}