using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class RecruiterDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid CompanyId { get; set; }
        public RecruiterPosition Position { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
