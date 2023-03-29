using FitnessApi.Entities;
using System.Diagnostics;
using System.Security.Claims;

namespace FitnessApi
{
    public static class AppDbContext
    {
        public static List<User> Users = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "Alex",
                Email = "goodStudent@email.com",
                UserName = "user",
                Password= "password"
            },
            new User
            {
                Id = 2,
                Name = "Nicu",
                Email = "badStudent@email.com",
                UserName = "student",
                Password = "pass"
                        
            },
            new User
            {
                Id = 3,
                Name = "Alina",
                Email = "mediocreStudent@email.com",
                UserName = "student23",
                Password = "pass1"
            }
        };
    }
}