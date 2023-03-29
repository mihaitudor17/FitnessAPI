using Core.Dtos;
using FitnessApi.Dtos;
using FitnessApi.Entities;
using FitnessApi.Mappings;
using FitnessApi.Repositories;

namespace FitnessApi.Services
{
    public class UserService
    {
        private UserRepository UserRepository { get; set; }

        public UserService(UserRepository studentsRepository)
        {
            this.UserRepository = studentsRepository;
        }

        public List<User> GetAll()
        {
            var results = UserRepository.GetAll();

            return results;
        }

        public UserDto GetById(int studentId)
        {
            var student = UserRepository.GetById(studentId);

            var result = student.ToUserDto();

            return result;
        }

        public bool EditPassword(UserUpdateDto payload)
        {
            if (payload == null || payload.Password == null || payload.UserName == null)
            {
                return false;
            }

            var result = UserRepository.GetById(payload.Id);
            if (result == null) return false;
            result.UserName = payload.UserName;
            result.Password = payload.Password;
            return true;
        }
        public UserDto DeleteUser(int userId)
        {
            var result = UserRepository.DeleteUser(userId);
            return result.ToUserDto();
        }
    }
}
