using System;
using System.Collections.Generic;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public Country? Country { get; set; }
        public string Company { get; set; }
        public Profile? Profile { get; set; }
        public Seniority? Seniority { get; set; }
        public JobTechLanguage? Stack { get; set; }
        public ICollection<Rekommendation> Rekommendations { get; set; } = new List<Rekommendation>();
    }
}
