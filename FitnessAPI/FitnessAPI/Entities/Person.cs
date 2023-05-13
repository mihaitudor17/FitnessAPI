namespace FitnessAPI.Entities
{
    public class Person : BaseEntity
    {
        private string name;
        private int height;
        private int weight;
        private int age;
        private bool gender;
        public ICollection<int> WorkoutIds { get; set; }
    }
}
