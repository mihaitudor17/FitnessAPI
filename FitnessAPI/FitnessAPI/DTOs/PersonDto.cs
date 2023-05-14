using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.DTOs
{
    public class PersonDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; }
    }
}
