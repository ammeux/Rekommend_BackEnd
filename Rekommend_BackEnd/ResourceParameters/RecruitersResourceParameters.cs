
namespace Rekommend_BackEnd.ResourceParameters
{
    public class RecruitersResourceParameters : ResourceParametersAbstract
    {
        public string RecruiterPosition { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyId { get; set; }
        public string OrderBy { get; set; } = "LastName";
    }
}
