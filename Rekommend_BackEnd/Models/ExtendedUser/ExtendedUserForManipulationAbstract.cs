using System;
using System.ComponentModel.DataAnnotations;
using static Rekommend_BackEnd.Utils.RekomEnums;

namespace Rekommend_BackEnd.Models
{
    public abstract class ExtendedUserForManipulationAbstract
    {
        [Required]
        public string Position { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
