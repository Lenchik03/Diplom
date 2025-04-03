using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemAPI.DTO
{
    public partial class UserDTO
    {
        public string FIO { get => $"{FirstName} {LastName} {Patronymic}"; }
    }
}
