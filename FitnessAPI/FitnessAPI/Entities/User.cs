using FitnessAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAPI.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password {get; set; }
        [ForeignKey(nameof(PersonId))]
        public int PersonId { get; set; }
        [Required]
        private RoleType role;
    }
}
