using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WaterTreatment.Web.Extensions
{
    static public class Extensions
    {
        static public IQueryable<T> SortBy<T, Key>(this IQueryable<T> query, Expression<Func<T, Key>> exp, bool descending)
            where T : class
            where Key : class
        {
            return descending ? query.OrderByDescending(exp) : query.OrderBy(exp);
        }

        static public IQueryable<T> SortBy<T>(this IQueryable<T> query, Expression<Func<T, DateTime>> exp, bool descending)
            where T : class
            {
            return descending ? query.OrderByDescending(exp) : query.OrderBy(exp);
        }
    }
}