using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.Entities
{
    public class Person : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public int Weight {get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender{get; set; }
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
