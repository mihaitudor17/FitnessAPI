using FitnessAPI.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessAPI.Entities
{
    public class Workout:BaseEntity
    {
        [Required]
        public WorkoutType Exercise { get; set; }
        [Required]
        public IntensityType Intensity { get; set; }
        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        public int Duration { get; set; }
        [NotMapped]
        private Dictionary<IntensityType, double> IntensityDict = new Dictionary<IntensityType, double>
        {
            { IntensityType.Easy, 0.5 },
            { IntensityType.Normal, 1 },
            { IntensityType.Hard, 1.5 }
        };
    }
}
