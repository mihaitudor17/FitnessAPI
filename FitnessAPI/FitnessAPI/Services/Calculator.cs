using FitnessAPI.Entities;

namespace FitnessAPI.Services
{
    public static class Calculator
    {
        private static double GetBMRCalories (Person person)
        {
            if(person.Gender == true)
            {
                return 66 + (6.2 * person.Weight) + (12.7 * person.Height) - (6.76 * person.Age);
            }
            else
            {
                return 655.1 + (4.35 * person.Weight) + (4.7 * person.Height) - (4.7 * person.Age);
            }
        }

        private static double GetWorkoutCalories(Workout workout)
        {
            return (workout.Duration / 60) * ((int)workout.Exercise * workout.IntensityDict[workout.Intensity]); 
        }

        public static double GetTotalCalories(Person person)
        {
            return person.Workouts.Sum(w => GetWorkoutCalories(w)) + GetBMRCalories(person);
        }
    }
}
