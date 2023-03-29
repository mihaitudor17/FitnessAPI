using FitnessApi.Dtos;
using FitnessApi.Entities;
using System.Diagnostics;

namespace FitnessApi.Mappings
{
    public static class UserMappings
    {
        public static List<UserDto> ToUserDtos(this List<User> Users)
        {
            var results = Users.Select(e => e.ToUserDto()).ToList();

            return results;
        }

        public static UserDto ToUserDto(this User User)
        {
            if (User == null) return null;

            var result = new UserDto();
            result.UserName = User.UserName;
            result.Password = User.Password;

            return result;
        }
    }
}
