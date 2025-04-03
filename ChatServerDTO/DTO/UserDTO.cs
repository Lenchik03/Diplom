using ProjectSystemAPI.DB;

namespace ProjectSystemAPI.DTO
{
    public partial class UserDTO
    {
        public int Id { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public int IdRole { get; set; }
        public string? RoleTitle { get; set; }

        public int IdDepartment { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Post { get; set; }

        public static explicit operator UserDTO(User from)
        {
            var result = new UserDTO
            {
                Birthday = from.Birthday,
                FirstName = from.FirstName,
                LastName = from.LastName,
                Email = from.Email,
                Password = from.Password,
                IdRole = from.IdRole,
                IdDepartment = from.IdDepartment,
                Phone = from.Phone,
                Id = from.Id,
                Patronymic = from.Patronymic,
                Post = from.Post,
            };

            if (from.IdRoleNavigation != null ) 
                result.RoleTitle = from.IdRoleNavigation.Title;

            return result;
        }

        public static explicit operator User(UserDTO from)
        {
            var result = new User
            {
                Birthday = from.Birthday,
                FirstName = from.FirstName,
                LastName = from.LastName,
                Email = from.Email,
                Password = from.Password,
                IdRole = from.IdRole,
                IdDepartment = from.IdDepartment,
                Phone = from.Phone,
                Id = from.Id,
                Patronymic = from.Patronymic,
                Post = from.Post,
            };

            return result;
        }
    }
}
