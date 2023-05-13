using FitnessAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FitnessAPI.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password {get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(Person))]
        public int PersonId { get; set; }
        [Required]
        private RoleType role;
    }
}
