using FitnessAPI.Enums;

namespace FitnessAPI.Entities
{
    public class Workout
    {
        public int WorkoutId { get; set; }
        private WorkoutType exercise;
        private IntensityType intensity;
        private DateOnly date;
        private int duration;
        public Workout(WorkoutType exercise, IntensityType intensity, int duration)
        {
            this.exercise = exercise;
            this.intensity = intensity;
            this.date = DateOnly.FromDateTime(DateTime.Now);
            this.duration = duration;
        }
        private Dictionary<IntensityType, double> IntensityDict = new Dictionary<IntensityType, double>
        {
            { IntensityType.Easy, 0.5 },
            { IntensityType.Normal, 1 },
            { IntensityType.Hard, 1.5 }
        };
    }
}
