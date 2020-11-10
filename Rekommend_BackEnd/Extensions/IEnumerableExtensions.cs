using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Rekommend_BackEnd.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // create a list to hold ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // create a list of PropertyInfo objects on TSource. Reflection is expensive, so rather than doing if for each object in the list
            // we do it once and reuse the results. After all, part of the reflection is on the type of the object
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach(var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object. We need to include public and instance, b/c specifying a binding flag overwite the already existing binding flags
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    
                    if(propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} was not found on {typeof(TSource)}");
                    }

                    // add propertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach(TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach(var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}
