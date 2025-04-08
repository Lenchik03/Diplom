using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSystemWPF.ViewModel
{
    public class TransferUserVM:BaseVM
    {
        public event EventHandler Loaded;
        ObservableCollection<DepartmentDTO> allDepartments = new();
        ObservableCollection<Role> allRoles = new();
        private UserDTO employee;
        private ObservableCollection<DepartmentDTO> departments = new();
        private ObservableCollection<Role> roles = new();

        public ObservableCollection<DepartmentDTO> Departments { get => departments; set
            {
                departments = value;
                Signal();
            }
        }
        public DepartmentDTO SelectedDep { get; set; }
        public ObservableCollection<Role> Roles
        {
            get => roles;
            set { roles = value;Signal(); }
        }
        public Role SelectedRole { get; set; }
        public VmCommand Save {  get; set; }

        public UserDTO Employee { get => employee; set
            {
                employee = value;
                Signal();
            }
        }
        public TransferUserVM()
        {
            GetLists();

            Save = new VmCommand(async () =>
            {
                var lastdepId = Employee.IdDepartment;
                DepartmentDTO department = Departments.FirstOrDefault(s => s.Id == lastdepId);
                department.Users.Remove(Employee);
                if (SelectedRole.Id == 1)
                {
                    if (SelectedDep.IdDirector == null)
                    {
                        SelectedDep.IdDirector = Employee.Id;
                        string arg = JsonSerializer.Serialize(SelectedDep, REST.Instance.options);
                        var responce = await REST.Instance.client.PutAsync($"Departmants/{SelectedDep.Id}",
                            new StringContent(arg, Encoding.UTF8, "application/json"));
                        try
                        {
                            responce.EnsureSuccessStatusCode();


                        }
                        catch (Exception ex)
                        {

                            return;
                        }
                        if (SelectedRole.Id == 1)
                        {
                            if (SelectedDep.ChildDepartments.Count == 0)
                            {
                                Employee.Id = 2;
                            }
                            else
                            {
                                Employee.Id = 1;
                            }
                        }                      

                       
                        Employee.IdDepartment = SelectedDep.Id;
                        string arg1 = JsonSerializer.Serialize(Employee, REST.Instance.options);
                        var responce1 = await REST.Instance.client.PutAsync($"Users/UpdateUser",
                            new StringContent(arg1, Encoding.UTF8, "application/json"));
                        try
                        {
                            responce1.EnsureSuccessStatusCode();
                            MessageBox.Show("Пользователь успешно обновлен!");
                            
                            string arg2 = JsonSerializer.Serialize(department, REST.Instance.options);
                            var responce2 = await REST.Instance.client.PutAsync($"Departmants/{department.Id}",
                                new StringContent(arg2, Encoding.UTF8, "application/json"));
                            try
                            {
                                responce2.EnsureSuccessStatusCode();


                            }
                            catch (Exception ex)
                            {

                                return;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("У этого отдела уже есть директор");
                        return;
                    }
                }
                else 
                {
                    Employee.IdRole = 3;
                    Employee.IdDepartment = SelectedDep.Id;
                    string arg1 = JsonSerializer.Serialize(Employee, REST.Instance.options);
                    var responce1 = await REST.Instance.client.PutAsync($"Users/UpdateUser",
                        new StringContent(arg1, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce1.EnsureSuccessStatusCode();
                        MessageBox.Show("Пользователь успешно обновлен!");

                        string arg = JsonSerializer.Serialize(department, REST.Instance.options);
                        var responce = await REST.Instance.client.PutAsync($"Departmants/{department.Id}",
                            new StringContent(arg, Encoding.UTF8, "application/json"));
                        try
                        {
                            responce.EnsureSuccessStatusCode();


                        }
                        catch (Exception ex)
                        {

                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                        return;
                    }
                }            
            });
            
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
                    Departments = await result.Content.ReadFromJsonAsync<ObservableCollection<DepartmentDTO>>(REST.Instance.options);
                }

                var result2 = await REST.Instance.client.GetAsync("Roles");
                if (result2.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    var test = await result2.Content.ReadAsStringAsync();
                    allRoles = await result2.Content.ReadFromJsonAsync<ObservableCollection<Role>>(REST.Instance.options);
                    Roles = new ObservableCollection<Role>(allRoles.Where(s => s.Id == 1 || s.Id == 3));
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
