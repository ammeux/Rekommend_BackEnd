using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HqCity { get; set; }
        public int PostCode { get; set; }
        public string HqCountry { get; set; }
        public string CompanyDescription { get; set; }
        public string Category { get; set; }
        public string LogoFileName { get; set; }
        public string Website { get; set; }
        public string EmployerBrandWebsite { get; set; }
    }
}
