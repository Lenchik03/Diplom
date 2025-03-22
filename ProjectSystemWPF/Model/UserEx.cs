using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.Model
{
    public partial class User
    {
        public string FIO { get => $"{FirstName} {LastName} {Patronymic}"; }
    }
}
