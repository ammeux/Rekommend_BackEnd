namespace Rekommend_BackEnd.ResourceParameters
{
    public class RekommendationsResourceParameters : ResourceParametersAbstract
    {
        public string TechJobOpeningId { get; set; }
        public string Position { get; set; }
        public string Seniority { get; set; }
        public string Status { get; set; }
        public bool HasAlreadyWorkedWithRekommender { get; set; }
        public string OrderBy { get; set; } = "Status";
    }
}
