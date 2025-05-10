using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using ProjectSystemWPF.View;
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
        ObservableCollection<ProjectDTO> projects = new();
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
                if (MessageBox.Show("Перевод сотрудника", "Вы уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var lastdepId = Employee.IdDepartment;
                    DepartmentDTO department = Departments.FirstOrDefault(s => s.Id == lastdepId);
                    if (Employee.IdRole == 1 || Employee.IdRole == 2)
                    {
                        department.IdDirector = null;
                        department.Director = null;
                    }

                    department.Users.Remove(Employee);
                    if (SelectedRole.Id == 1)
                    {
                        if (SelectedDep.IdDirector == null)
                        {
                            SelectedDep.IdDirector = Employee.Id;
                            string arg = JsonSerializer.Serialize(SelectedDep, REST.Instance.options);
                            var responce = await REST.Instance.client.PutAsync($"Departments/{SelectedDep.Id}",
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
                                var responce2 = await REST.Instance.client.PutAsync($"Departments/{department.Id}",
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
                        if (SelectedDep != null && Employee.IdDepartment != SelectedDep.Id)
                        {


                            var userprojects = projects.Where(s => s.IdCreator == Employee.Id).ToList();
                            GetDefaultUser();
                            if (userprojects.Count > 0)
                            {
                                foreach (var project in userprojects)
                                {
                                    project.IdCreator = 59;
                                    project.Creator = defaultUser;
                                    string arg = JsonSerializer.Serialize(project, REST.Instance.options);
                                    var responce = await REST.Instance.client.PutAsync($"Projects/{project.Id}",
                                        new StringContent(arg, Encoding.UTF8, "application/json"));
                                    try
                                    {
                                        responce.EnsureSuccessStatusCode();
                                        //MessageBox.Show("Пользователь успешно обновлен!");

                                    }
                                    catch (Exception ex)
                                    {
                                        //MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                                        return;
                                    }
                                }
                            }
                        }
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
                            var responce = await REST.Instance.client.PutAsync($"Departments/{department.Id}",
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
                    transferUserWindow.Close();
                }
            });
            
        }

        UserDTO defaultUser = null;
        public async void GetDefaultUser()
        {
            var result = await REST.Instance.client.GetAsync($"Users/{59}");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                defaultUser = await result.Content.ReadFromJsonAsync<UserDTO>(REST.Instance.options);
            }
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

                var result3 = await REST.Instance.client.GetAsync("Projects");
                //todo not ok

                if (result3.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    projects = await result3.Content.ReadFromJsonAsync<ObservableCollection<ProjectDTO>>(REST.Instance.options);
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

        TransferUserWindow transferUserWindow;
        internal void GetWindow(TransferUserWindow transferUserWindow)
        {
            this.transferUserWindow = transferUserWindow;
        }
    }
}
