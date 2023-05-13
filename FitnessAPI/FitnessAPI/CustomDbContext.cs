using FitnessAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessAPI
{
    public class CustomDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }
    }
}
