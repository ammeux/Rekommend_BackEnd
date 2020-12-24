using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class RekommenderDto
    {
        public Guid Id { get; set; }
        public int Age { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Seniority { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public int PostCode { get; set; }
        public int XpRekommend { get; set; }
        public int RekommendationsAvgGrade { get; set; }
        public string Level { get; set; }
    }
}
