using System;

namespace Rekommend_BackEnd.Models
{
    public class ExtendedUserDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Position { get; set; }
        public int Age { get; set; }
    }
}
