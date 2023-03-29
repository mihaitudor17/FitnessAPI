using FitnessApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace FitnessApi.Repositories
{
    public class UserRepository
    {
        public List<User> GetAll()
        {
            var results = AppDbContext.Users;

            return results;
        }

        public User GetById(int userId)
        {
            var result = AppDbContext.Users
                .Where(e => e.Id == userId)
                .FirstOrDefault();

            return result;
        }

        public User GetByIdWithEmail(int userId, string email)
        {
            var result = AppDbContext.Users
               .Select(e => new User
               {
                   Name = e.Name,
                   UserName = e.UserName,
                   Id = e.Id,
                   Password = e.Password,
                   Email = e.Email

               })
               .Where(g => g.Email == email && g.Id == userId)
                        .OrderByDescending(g => g.Name)
                        .ToList()
               .FirstOrDefault(e => e.Id == userId);

            return result;
        }

        public User DeleteUser(int userId)
        {
            var result = AppDbContext.Users
           .FirstOrDefault(e => e.Id == userId);
            if (result != null)
            {
                AppDbContext.Users.Remove(result);
                return result;
            }

            return null;
        }
    }
}

