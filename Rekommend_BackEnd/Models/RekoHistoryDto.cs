using System;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public class RekoHistoryDto
    {
        public Guid Id { get; set; }
        public Guid RekommenderId { get; set; }
        public Guid RekommendationId { get; set; }
        public DateTimeOffset Date { get; set; }
        public RekommendationStatus RekommendationStatus { get; set; }
    }
}
