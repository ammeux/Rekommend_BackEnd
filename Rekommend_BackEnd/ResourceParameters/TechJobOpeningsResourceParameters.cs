using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.ResourceParameters
{
    public class TechJobOpeningsResourceParameters
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;
        public string JobTechLanguage { get; set; }
        public string CompanyCategory { get; set; }
        public string City { get; set; }
        public string Seniority { get; set; }
        public string JobPosition { get; set; }
        public bool RemoteWorkAccepted { get; set; }
        public string ContractType { get; set; }
        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public string OrderBy { get; set; } = "CreationDate";
        public string Fields { get; set; }
    }
}
