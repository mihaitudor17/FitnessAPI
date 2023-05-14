using FitnessAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.DTOs
{
    public class WorkoutDto
    {
        [Required]
        public WorkoutType Exercise { get; set; }
        [Required]
        public IntensityType Intensity { get; set; }
        [Required]
        public int Duration { get; set; }
    }
}
