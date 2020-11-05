using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class RekommendationDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset StatusChangeDate { get; set; }
        public Guid RekommenderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Position Position { get; set; }
        public Seniority Seniority { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public RekommendationStatus Status { get; set; }
    }
}
