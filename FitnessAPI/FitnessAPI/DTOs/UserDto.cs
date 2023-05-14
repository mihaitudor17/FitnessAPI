using FitnessAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.DTOs
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public RoleType Role { get; set; }
    }
}
