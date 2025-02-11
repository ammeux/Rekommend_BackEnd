﻿using Rekommend_BackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Rekommend_BackEnd.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if(mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
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
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Property {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if(propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                foreach(var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    if(propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    orderByString = orderByString + (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ") + propertyName + (orderDescending ? " descending" : " ascending");
                }

                
            }
            return source.OrderBy(orderByString);
        }
    }
}
