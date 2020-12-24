namespace Rekommend_BackEnd.ResourceParameters
{
    public class RekommendersResourceParameters : ResourceParametersAbstract
    {
        public string Position { get; set; }
        public string Seniority { get; set; }
        public string Company { get; set; }
        public int XpRekommend { get; set; }
        public int RekommendationsAvgGrade { get; set; }
        public string Level { get; set; }
        public string OrderBy { get; set; } = "RekommendationsAvgGrade";
    }
}
