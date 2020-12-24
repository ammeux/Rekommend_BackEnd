using System;

namespace Rekommend_BackEnd.Models
{
    public class RecruiterDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid CompanyId { get; set; }
        public string Position { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }
}
