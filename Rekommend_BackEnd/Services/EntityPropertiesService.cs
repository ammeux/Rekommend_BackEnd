using Rekommend_BackEnd.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rekommend_BackEnd.Services
{
    public class EntityPropertiesService : IEntityPropertiesService
    {
        public bool TryGetPropertiesHash(string entityType, out HashSet<string> properties)
        {
            properties = null;
            if (entityType == "TechJobOpening")
            {
                System.Type type = typeof(TechJobOpening);
                IEnumerable<PropertyInfo> propertiesEnum = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).AsEnumerable();
                properties = new HashSet<string>();
                foreach (var property in propertiesEnum)
                {
                    properties.Add(property.Name);
                }

                return true;
            }
            return false;
        }
    }
}
