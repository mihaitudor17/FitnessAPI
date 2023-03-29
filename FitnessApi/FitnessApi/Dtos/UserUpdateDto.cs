using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class UserUpdateDto
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; }

        [Required, MaxLength(100)]
        public string Password { get; set; }
    }
}
