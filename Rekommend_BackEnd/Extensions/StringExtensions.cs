using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Extensions
{
    public static class StringExtensions
    {
        public static CompanyCategory ToCompanyCategory(this string source)
        {
            if(source == null)
            {
                return CompanyCategory.Undefined;
            }
            switch (source.ToLower())
            {
                case "undefined":
                    return CompanyCategory.Undefined;
                case "ecology":
                    return CompanyCategory.Ecology;
                case "localfood":
                    return CompanyCategory.LocalFood;
                case "health":
                    return CompanyCategory.Health;
                case "silvereconomy":
                    return CompanyCategory.SilverEconomy;
                default:
                    return CompanyCategory.Undefined;
            }
        }

        public static JobTechLanguage ToJobTechLanguage(this string source)
        {
            if (source == null)
            {
                return JobTechLanguage.Undefined;
            }

            switch (source.ToLower())
            {
                case "undefined":
                    return JobTechLanguage.Undefined;
                case "javascript":
                    return JobTechLanguage.JavaScript;
                case "python":
                    return JobTechLanguage.Python;
                case "cs":
                    return JobTechLanguage.CS;
                case "java":
                    return JobTechLanguage.Java;
                default:
                    return JobTechLanguage.Undefined;
            }
        }

        public static Country ToCountry(this string source)
        {
            if (source == null)
            {
                return Country.Undefined;
            }

            switch (source.ToLower())
            {
                case "undefined":
                    return Country.Undefined;
                case "france":
                    return Country.France;
                case "other":
                    return Country.Other;
                default:
                    return Country.Undefined;
            }
        }

        public static ContractType ToContractType(this string source)
        {
            if (source == null)
            {
                return ContractType.Undefined;
            }

            switch (source.ToLower())
            {
                case "undefined":
                    return ContractType.Undefined;
                case "cdi":
                    return ContractType.CDI;
                case "cdd":
                    return ContractType.CDD;
                case "internship":
                    return ContractType.Internship;
                case "workstudy":
                    return ContractType.WorkStudy;
                case "freelance":
                    return ContractType.Freelance;
                default:
                    return ContractType.Undefined;
            }
        }

        public static Position ToJobPosition(this string source)
        {
            if (source == null)
            {
                return Position.Undefined;
            }

            switch (source.ToLower())
            {
                case "undefined":
                    return Position.Undefined;
                case "techleader":
                    return Position.TechLeader;
                case "developer":
                    return Position.Developer;
                case "businessanalyst":
                    return Position.BusinessAnalyst;
                case "productowner":
                    return Position.ProductOwner;
                case "qa":
                    return Position.QA;
                case "designer":
                    return Position.Designer;
                default:
                    return Position.Undefined;
            }
        }

        public static Seniority ToSeniority(this string source)
        {
            if (source == null)
            {
                return Seniority.Undefined;
            }

            switch (source.ToLower())
            {
                case "undefined":
                    return Seniority.Undefined;
                case "junior":
                    return Seniority.Junior;
                case "senior":
                    return Seniority.Senior;
                case "director":
                    return Seniority.Director;
                case "executive":
                    return Seniority.Executive;
                default:
                    return Seniority.Undefined;
            }
        }

        public static RecruiterPosition ToRecruiterPosition(this string source)
        {
            if(source == null)
            {
                return RecruiterPosition.Undefined;
            }

            switch(source.ToLower())
            {
                case "undefined":
                    return RecruiterPosition.Undefined;
                case "founder":
                    return RecruiterPosition.Founder;
                case "talentacquisitionmanager":
                    return RecruiterPosition.TalentAcquisitionManager;
                case "other":
                    return RecruiterPosition.Other;
                default:
                    return RecruiterPosition.Undefined;
            }
        }
    }
}
