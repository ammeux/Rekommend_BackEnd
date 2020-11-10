using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Utils;

namespace Rekommend_BackEnd.Repositories
{
    public interface IRekommendRepository
    {
        PagedList<TechJobOpening> GetTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters);
    }
}
