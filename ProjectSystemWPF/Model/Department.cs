using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.Model
{
    public partial class Department
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int IdMainDep { get; set; }

        public int? IdDirector { get; set; }

        public virtual Department IdMainDepNavigation { get; set; } = null!;

        public virtual ICollection<Department> InverseIdMainDepNavigation { get; set; } = new List<Department>();

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }

}
