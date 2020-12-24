namespace Rekommend_BackEnd.ResourceParameters
{
    public class TechJobOpeningsResourceParameters : ResourceParametersAbstract
    {
        public string JobTechLanguage { get; set; }
        public string CompanyCategory { get; set; }
        public string City { get; set; }
        public int PostCode { get; set; }
        public string Seniority { get; set; }
        public string JobPosition { get; set; }
        public bool RemoteWorkAccepted { get; set; }
        public string ContractType { get; set; }
        public string OrderBy { get; set; } = "CreationDate";
    }
}
