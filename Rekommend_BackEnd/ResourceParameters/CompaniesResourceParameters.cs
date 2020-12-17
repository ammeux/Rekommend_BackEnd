
namespace Rekommend_BackEnd.ResourceParameters
{
    public class CompaniesResourceParameters : ResourceParametersAbstract
    {
        public string Name { get; set; }
        public string HqCity { get; set; }
        public string HqCountry { get; set; }
        public string Category { get; set; }
        public string OrderBy { get; set; } = "Name";
    }
}
