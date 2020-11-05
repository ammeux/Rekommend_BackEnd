using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class RekommenderDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Position Position { get; set; }
        public Seniority Seniority { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
    }
}
