using System;

namespace Rekommend_BackEnd.Models
{
    public class RekommendationDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset StatusChangeDate { get; set; }

        public Guid RekommenderId { get; set; }
        public string RekommenderFirstName { get; set; }
        public string RekommenderLastName { get; set; }
        public string RekommenderPosition { get; set; }
        public string RekommenderSeniority { get; set; }
        public string RekommenderCompany { get; set; }
        public string RekommenderEmail { get; set; }
        public string RekommenderCity { get; set; }
        public int RekommenderPostCode { get; set; }

        public Guid TechJobOpeningId { get; set; }
        public string TechJobOpeningTitle { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Seniority { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public bool HasAlreadyWorkedWithRekommender { get; set; }
    }
}
