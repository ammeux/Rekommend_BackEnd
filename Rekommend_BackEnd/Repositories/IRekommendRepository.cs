using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.ResourceParameters;
using Rekommend_BackEnd.Utils;
using System;

namespace Rekommend_BackEnd.Repositories
{
    public interface IRekommendRepository
    {
        PagedList<TechJobOpening> GetTechJobOpenings(TechJobOpeningsResourceParameters techJobOpeningsResourceParameters);
        Recruiter GetRecruiter(Guid recruiterId);
        Company GetCompany(Guid companyId);
    }
}
