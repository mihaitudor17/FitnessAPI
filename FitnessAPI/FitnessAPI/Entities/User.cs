using FitnessAPI.Enums;

namespace FitnessAPI.Entities
{
    public class User : BaseEntity
    {
        private string userName;
        private string password;
        public int PersonId { get; set; }
        private RoleType role;
    }
}
