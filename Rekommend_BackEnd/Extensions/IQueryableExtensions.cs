using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Rekommend_BackEnd.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, HashSet<string> propertiesHash)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if(propertiesHash == null)
            {
                throw new ArgumentNullException(nameof(propertiesHash));
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            // the orderBy string is separated by "," so we split it
            var orderByAfterSplit = orderBy.Split(',');

            // apply each orderByClause
            foreach (var orderByClause in orderByAfterSplit)
            {
                var trimmedOrderByClause = orderByClause.Trim();

                // if the sort option ends with with " desc", we order
                // descending, ortherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove " asc" or " desc" from the orderBy clause, so we 
                // get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertiesHash.Contains(propertyName))
                {
                    throw new ArgumentException($"Property {propertyName} is missing");
                }

                orderByString = orderByString + (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ") + propertyName + (orderDescending ? " descending" : " ascending");
            }
            return source.OrderBy(orderByString);
        }
    }
}
