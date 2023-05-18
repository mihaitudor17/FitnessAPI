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
        public string Role { get; set; }
        public RoleType GetRoleType()
        {
            if (Enum.TryParse<RoleType>(Role, out var roleType))
            {
                return roleType;
            }
            // Default value if parsing fails
            return RoleType.User;
        }
    }
}
