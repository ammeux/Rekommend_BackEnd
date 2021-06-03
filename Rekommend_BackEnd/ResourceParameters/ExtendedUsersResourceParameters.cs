
namespace Rekommend_BackEnd.ResourceParameters
{
    public class ExtendedUsersResourceParameters : ResourceParametersAbstract
    {
        public string RecruiterPosition { get; set; }
        public string CompanyId { get; set; }
        public string OrderBy { get; set; } = "LastName";
    }
}
