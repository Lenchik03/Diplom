using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class TransferUserVM
    {
        public event EventHandler Loaded;
        ObservableCollection<DepartmentDTO> allDepartments = new();
        public ObservableCollection<DepartmentDTO> Departments { get; set; } = new();
        public DepartmentDTO SelectedDep { get; set; }
        public ObservableCollection<Role> Roles { get; set; } = new();
        public Role SelectedRole { get; set; }
        public VmCommand Save {  get; set; }

        public UserDTO Employee { get; set; }
        public TransferUserVM()
        {
            GetLists();
        }

        public async void GetLists()
        {
            try
            {
                var result = await REST.Instance.client.GetAsync("Departments");
                //todo not ok

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    allDepartments = await result.Content.ReadFromJsonAsync<ObservableCollection<DepartmentDTO>>(REST.Instance.options);
                }

                var result2 = await REST.Instance.client.GetAsync("Roles");
                if (result2.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    var test = await result2.Content.ReadAsStringAsync();
                    Roles = await result2.Content.ReadFromJsonAsync<ObservableCollection<Role>>(REST.Instance.options);
                }
                Loaded?.Invoke(this, null);
            }
            catch (Exception ex)
            {
            }
        }

        internal void GetUser(UserDTO user)
        {
            Employee = user;
        }
    }
}
