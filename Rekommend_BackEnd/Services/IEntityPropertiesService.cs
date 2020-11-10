using System.Collections.Generic;

namespace Rekommend_BackEnd.Services
{
    public interface IEntityPropertiesService
    {
        bool TryGetPropertiesHash(string entityType, out HashSet<string> properties);
    }
}
