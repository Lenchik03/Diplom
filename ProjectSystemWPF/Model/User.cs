using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.Model
{
    public partial class User
    {
        public int Id { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int IdRole { get; set; }

        public int IdDepartment { get; set; }

        public string Phone { get; set; } = null!;

        public DateOnly? Birthday { get; set; }

        public virtual Department IdDepartmentNavigation { get; set; } = null!;

        public virtual Role IdRoleNavigation { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
