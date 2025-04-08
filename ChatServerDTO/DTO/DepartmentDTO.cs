using ProjectSystemAPI.DB;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DTO
{
    public class DepartmentDTO : INotifyPropertyChanged
    {
        private bool selected;

        public int Id { get; set; }

        public string? Title { get; set; }

        public int? IdMainDep { get; set; }

        public int? IdDirector { get; set; }

        public UserDTO? Director { get; set; }
        public List<UserDTO>? Users { get; set; }
        public List<DepartmentDTO>? ChildDepartments { get; set; }
        protected void Signal([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        [JsonIgnore]
        public bool Selected 
        { get => selected;
            set { 
                selected = value;
                Signal();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static explicit operator DepartmentDTO(Department from)
        {
            var result = new DepartmentDTO
            {
                Id = from.Id,
                IdDirector = from.IdDirector,
                IdMainDep = from.IdMainDep,
                Title = from.Title
            };

            if (from.Users != null)
                result.Users = from.Users.Select(s => (UserDTO)s).ToList();

            if (from.IdDirectorNavigation != null)
                result.Director = (UserDTO)from.IdDirectorNavigation;

            if (from.InverseIdMainDepNavigation != null)
                result.ChildDepartments = from.InverseIdMainDepNavigation.Select(s => (DepartmentDTO)s).ToList();

            return result;
        }

        public static explicit operator Department(DepartmentDTO from)
        {
            return new Department
            {
                Id = from.Id,
                IdDirector = from.IdDirector,
                IdMainDep = from.IdMainDep,
                Title = from.Title
            };
        }
    }
}
