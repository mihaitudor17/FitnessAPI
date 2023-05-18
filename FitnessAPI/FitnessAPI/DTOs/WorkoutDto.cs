using FitnessAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessAPI.DTOs
{
    public class WorkoutDto
    {
        [Required]
        public string Exercise { get; set; }
        [Required]
        public string Intensity { get; set; }
        [Required]
        public int Duration { get; set; }

        public WorkoutType GetExerciseType()
        {
            if (Enum.TryParse<WorkoutType>(Exercise, out var exerciseType))
            {
                return exerciseType;
            }
            return WorkoutType.Jogging;
        }

        public IntensityType GetIntensityType()
        {
            if (Enum.TryParse<IntensityType>(Intensity, out var intensityType))
            {
                return intensityType;
            }
            return IntensityType.Normal;
        }
    }
}
