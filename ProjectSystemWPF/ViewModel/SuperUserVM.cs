using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using MaterialDesignColors;
using MaterialDesignColors.Recommended;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using ProjectSystemWPF.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Runtime.ExceptionServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectSystemWPF.ViewModel
{
    public class SuperUserVM : BaseVM
    {
        public ObservableCollection<DepartmentDTO> MainDepartments { get; set; } = new();
        public ObservableCollection<DepartmentDTO> Departments { get; set; } = new();
        public ObservableCollection<Role> Roles { get; set; } = new();
        public ObservableCollection<UserDTO> Employees
        {
            get => employees;
            set { employees = value; Signal(); }

        }
        ObservableCollection<DepartmentDTO> allDepartments = new();
        ObservableCollection<ProjectDTO> projects = new();
        ObservableCollection<UserDTO> allEmployees = new();
        public bool CanEdit
        {
            get => canEdit;
            set
            {
                canEdit = value;
                Signal();
            }
        }
        public VmCommand NewEmployee { get; set; }
        public UserDTO Employee
        {
            get => employee;
            set
            {
                employee = value;
                Signal();
            }

        }
        public VmCommand SaveUser { get; set; }
        public Visibility Hidden
        {
            get => hidden;
            set
            {
                hidden = value;
                Signal();
            }
        }
        public Visibility HiddenEditUser
        {
            get => hiddenEditUser;
            set
            {
                hiddenEditUser = value;
                Signal();
            }
        }

        public Visibility TransferVisible
        {
            get => transferVisible;
            set { transferVisible = value;
                Signal();
            }
        }
        public Visibility HiddenEditDep
        {
            get => hiddenEditDep;
            set
            {
                hiddenEditDep = value;
                Signal();
            }
        }
        public VmCommand CanEditClick { get; set; }
        public bool CanEditDep
        {
            get => canEditDep;
            set
            {
                canEditDep = value;
                Signal();
            }
        }
        public VmCommand NewDep { get; set; }
        public DepartmentDTO Department
        {
            get => department;
            set
            {
                department = value;
                SetHiddenButtons();
                Signal();
            }
        }
        public ObservableCollection<UserDTO> Directors
        {
            get => directors; set
            {
                directors = value;
                Signal();
            }
        }
        public UserDTO DepDirector
        {
            get => depDirector;
            set { depDirector = value; Signal(); }
        }
        public List<Expander> Expanders { get; set; } = new List<Expander>();

        public VmCommand SaveDep { get; set; }
        public VmCommand CanEditDepClick { get; set; }
        public Button SelectedDepOrUser { get; set; }
        public VmCommand DeleteEmployee { get; set; }
        public VmCommand DeleteDepartment { get; set; }
        public VmCommand test { get; set; }
        public VmCommand TransferClick { get; set; }

        public string Search
        {
            get => search;
            set
            {
                search = value;
                GetLists();
                
            }
        }

        Brush brush = new SolidColorBrush(Color.FromArgb(255, 223, 196, 01));
        private UserDTO employee = new UserDTO
        {
            Password = "",
            FirstName = "Нажмите на кнопку +",
            //IdRoleNavigation = new Role { Id = 3, Title = "" },
            //IdDepartmentNavigation = new DepartmentDTO { Id = 1, Title = "" }
        };
        private DepartmentDTO department;
        private ObservableCollection<UserDTO> employees;
        private UserDTO depDirector;
        private ObservableCollection<UserDTO> directors = new();
        private bool canEdit;
        private bool canEditDep;
        private Visibility hidden;
        private Visibility hiddenEditUser;
        private Visibility hiddenEditDep;
        private string search;


        public event EventHandler Loaded;

        void SetHiddenButtons()
        {
            TransferVisible = Visibility.Collapsed;
            Hidden = Visibility.Collapsed;
            HiddenEditUser = Visibility.Collapsed;
            HiddenEditDep = Visibility.Collapsed;
            var dep = allDepartments.FirstOrDefault(s => s.Id == ActiveUser.GetInstance().User.IdDepartment);
            if (ActiveUser.GetInstance().User.IdRole == 2 && (dep == Department || CheckSubDepartment(dep, Department)))
            {
                HiddenEditUser = Visibility.Visible;
                Hidden = Visibility.Visible;
            }
            
            if (ActiveUser.GetInstance().User.IdRole == 1 && (dep == Department || CheckSubDepartment(dep, Department)))
            {
                HiddenEditDep = Visibility.Visible;
                HiddenEditUser = Visibility.Visible;
                Hidden = Visibility.Visible;
            }

            if (ActiveUser.GetInstance().User.IdRole == 4)
            {
                HiddenEditDep = Visibility.Visible;
                HiddenEditUser = Visibility.Visible;
                Hidden = Visibility.Visible;
                TransferVisible = Visibility.Visible;
            }

        }

        public SuperUserVM()
        {
            GetLists();


            SetHiddenButtons();
            TransferClick = new VmCommand(async () =>
                {
                    TransferUserWindow transferUserWindow = new TransferUserWindow(Employee);
                    transferUserWindow.ShowDialog();
                    GetLists();
                    CreateExpanders();
                });
            test = new VmCommand(() =>
            {

                Expanders[1].IsExpanded = true;
            });
            DeleteEmployee = new VmCommand(async () =>
            {
                if (MessageBox.Show("Увольнение сотрудника", "Вы уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (Employee == null)
                        MessageBox.Show("Выберите сотрудника для увольнения!");
                    else
                    {
                        if (Employee.Id == Department.IdDirector)
                        {
                            Department.Director = null;
                            Department.IdDirector = null;
                            string arg1 = JsonSerializer.Serialize(Department, REST.Instance.options);
                            var responce1 = await REST.Instance.client.PutAsync($"Departments/{Department.Id}",
                                new StringContent(arg1, Encoding.UTF8, "application/json"));
                            try
                            {
                                responce1.EnsureSuccessStatusCode();
                                //MessageBox.Show("Пользователь успешно обновлен!");

                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                                return;
                            }

                        }
                        var userprojects = projects.Where(s => s.IdCreator == Employee.Id).ToList();
                        GetDefaultUser();
                        if (userprojects.Count > 0)
                        {
                            foreach (var project in userprojects)
                            {
                                project.IdCreator = 59;
                                project.Creator = defaultUser;
                                string arg1 = JsonSerializer.Serialize(project, REST.Instance.options);
                                var responce1 = await REST.Instance.client.PutAsync($"Projects/{project.Id}",
                                    new StringContent(arg1, Encoding.UTF8, "application/json"));
                                try
                                {
                                    responce1.EnsureSuccessStatusCode();
                                    //MessageBox.Show("Пользователь успешно обновлен!");

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                                    return;
                                }
                            }
                        }
                        Employee.IsDeleted = true;

                        string arg = JsonSerializer.Serialize(Employee, REST.Instance.options);
                        var responce = await REST.Instance.client.PutAsync($"Users/UpdateUser",
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

                        Employee = new UserDTO();

                    }
                    GetLists();
                    CreateExpanders();
                }
            });

            

            DeleteDepartment = new VmCommand(async () =>
            {
                if (MessageBox.Show("Удаление отдела", "Вы уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    if (Department == null)
                        MessageBox.Show("Выберите отдел для удаления!");
                    if (Department.ChildDepartments != null)
                    {
                        if (Department.ChildDepartments.Count != 0)
                        {
                            foreach (var dep in Department.ChildDepartments)
                            {
                                if (dep.Users.Count != 0)
                                {
                                    MessageBox.Show("Сначала переведите сотрудников в другие отделы или увольте их");
                                    return;
                                }
                                else
                                {
                                    Department.IsDeleted = true;
                                    string arg = JsonSerializer.Serialize(Department, REST.Instance.options);
                                    var responce = await REST.Instance.client.PutAsync($"Departments/{Department.Id}",
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

                                    Department = new DepartmentDTO();
                                }
                            }
                        }

                        else
                        {
                            if (Department.Users.Count != 0)
                                MessageBox.Show("Сначала переведите сотрудников в другие отделы или увольте их!");

                            else
                            {
                                Department.IsDeleted = true;
                                string arg = JsonSerializer.Serialize(Department, REST.Instance.options);
                                var responce = await REST.Instance.client.PutAsync($"Departments/{Department.Id}",
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
                                Department = new DepartmentDTO();
                            }
                        }
                    }
                    GetLists();
                    CreateExpanders();
                }
            });

            CanEditClick = new VmCommand(() =>
            {
                CanEdit = !CanEdit;
            });
            CanEditDepClick = new VmCommand(() =>
            {
                CanEditDep = !CanEditDep;
            });

            SaveUser = new VmCommand(async () =>
            {
                try
                {
                    if (Employee.FirstName != null && Employee.LastName != null && Employee.Patronymic != null && Employee.Email != null && Employee.Phone != null)
                    {


                        Regex r = new Regex("^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$");
                        Regex r2 = new Regex("^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[;]{0,1}\\s*)+$");
                        if (!r.Match(Employee.Phone).Success)
                        {
                            MessageBox.Show("Телефон должен состоять из цифр и символов “+”, “(”, “)”, “-”, “ ”, “#” ");
                            return;
                        }
                        if (!r2.Match(Employee.Email).Success)
                        {
                            MessageBox.Show("Почта имеет неправильный формат!");
                            return;
                        }
                        if (Employee != null && Employee.Id != 0)
                        {
                            //Employee.Departments = new ObservableCollection<DepartmentDTO>();
                            //Employee.IdDepartmentNavigation = new DepartmentDTO { Title = "", Id = Employee.IdDepartment};
                            //Employee.IdRoleNavigation = new Role { Title = "", Id = Employee.IdRole};
                            string arg = JsonSerializer.Serialize(Employee, REST.Instance.options);
                            var responce = await REST.Instance.client.PutAsync($"Users/UpdateUser",
                                new StringContent(arg, Encoding.UTF8, "application/json"));
                            try
                            {
                                responce.EnsureSuccessStatusCode();
                                MessageBox.Show("Пользователь успешно обновлен!");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                                return;
                            }
                        }
                        else
                        {
                            if (ActiveUser.GetInstance().User.IdRole == 4)
                            {
                                if (Department == null)
                                {
                                    MessageBox.Show("Выберите отдел, куда добавить сотрудника!");
                                    return;
                                }
                                else
                                {
                                    if (Department.IdMainDep != null)
                                        Department = MainDepartments.FirstOrDefault(s => s.Id == Department.IdMainDep);
                                    if (Department.IdDirector == null)
                                    {
                                        Employee.IdDepartment = Department.Id;
                                        //Employee.IdDepartmentNavigation = Department;
                                        Employee.IdRole = 1;
                                    }
                                    else
                                    {
                                        MessageBox.Show($"У отдела '{Department.Title}' уже есть руководитель!");
                                        return;
                                    }

                                    //Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 1);

                                    // Department.IdDirectorNavigation = Employee;
                                }

                            }
                            if (ActiveUser.GetInstance().User.IdRole == 1)
                            {
                                var shit = ActiveUser.GetInstance().User;
                                var department = allDepartments.FirstOrDefault(s => s.Id == ActiveUser.GetInstance().User.IdDepartment);
                                


                                    //if (ActiveUser.GetInstance().User.IdDepartmentNavigation.InverseIdMainDepNavigation.Count == 0)
                                    if (department.ChildDepartments.Count == 0)
                                    {
                                        Employee.IdRole = 3;
                                        // Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 3);
                                        Employee.IdDepartment = ActiveUser.GetInstance().User.IdDepartment;
                                        //Employee.IdDepartmentNavigation = ActiveUser.GetInstance().User.IdDepartmentNavigation;
                                    }
                                    else
                                    {
                                    if (Department.IdDirector == null)
                                    {


                                        Employee.IdRole = 2;
                                        //Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 2);
                                        Employee.IdDepartment = Department.Id;
                                        //Employee.IdDepartmentNavigation = Department;
                                    }
                                    else
                                    {
                                        MessageBox.Show($"У отдела '{Department.Title}' уже есть руководитель!");
                                        return;
                                    }
                                }

                                }
                                
                            
                            if (ActiveUser.GetInstance().User.IdRole == 2)
                            {
                                Employee.IdRole = 3;
                                //Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 3);
                                Employee.IdDepartment = ActiveUser.GetInstance().User.IdDepartment;
                                //Employee.IdDepartmentNavigation = ActiveUser.GetInstance().User.IdDepartmentNavigation;
                            }

                            //Employee.IdDepartmentNavigation = new DepartmentDTO
                            //{
                            //    IdMainDep =Employee.IdDepartmentNavigation?.IdMainDep,  
                            //    Id = Employee.IdDepartmentNavigation.Id, 
                            //    Title = Employee.IdDepartmentNavigation .Title};
                            //Employee.Password = "";
                            Employee.IsDeleted = false;
                            string arg = JsonSerializer.Serialize(Employee, REST.Instance.options);
                            var responce = await REST.Instance.client.PostAsync($"Users/AddNewUser",
                                new StringContent(arg, Encoding.UTF8, "application/json"));
                            //MessageBox.Show(responce.StatusCode.ToString());
                            try
                            {
                                responce.EnsureSuccessStatusCode();
                                    MessageBox.Show("Сотрудник был добавлен");
                                
                                    


                                if (Employee.IdRole == 1 || Employee.IdRole == 2)
                                {
                                    //var str = await responce.Content.ReadAsStringAsync();
                                    var answerUser = await responce.Content.ReadFromJsonAsync<UserDTO>(REST.Instance.options);
                                    Department.IdDirector = answerUser.Id;
                                    // Employee.IdDepartmentNavigation.IdDirector = answerUser.Id;


                                    string arg1 = JsonSerializer.Serialize(Department, REST.Instance.options);
                                    var responce1 = await REST.Instance.client.PutAsync($"Departments/{Department.Id}",
                                        new StringContent(arg1, Encoding.UTF8, "application/json"));
                                    try
                                    {
                                        responce1.EnsureSuccessStatusCode();
                                        //MessageBox.Show("Пользователь успешно обновлен!");

                                    }
                                    catch (Exception ex)
                                    {
                                        //MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(await responce.Content.ReadAsStringAsync());
                                return;
                            }


                        }

                        GetLists();
                        CreateExpanders();
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!");
                        return;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show("Ошибка!");
                }
            });

            SaveDep = new VmCommand(async () =>
            {
                Department.IsDeleted = false;
                if (Department.Id != 0 && Department != null)
                {
                    string arg = JsonSerializer.Serialize(Department, REST.Instance.options);
                    var responce = await REST.Instance.client.PutAsync($"Departments/{Department.Id}",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Отдел успешно обновлен!");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка! Обновление отдела приостановлено!");
                        return;
                    }


                }
                else
                {
                    if (ActiveUser.GetInstance().User.IdRole == 4)
                    {
                        //Department.IdMainDepNavigation = null;
                        Department.IdMainDep = null;
                    }
                    if (ActiveUser.GetInstance().User.IdRole == 1)
                    {
                        Department.IdMainDep = ActiveUser.GetInstance().User.IdDepartment;
                        //Department.IdMainDepNavigation = ActiveUser.GetInstance().User.IdDepartmentNavigation;
                    }

                    string arg = JsonSerializer.Serialize(Department, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Departments",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Отдел был добавлен!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка. Заполните все данные!");
                        return;
                    }
                }
                GetLists();
                CreateExpanders();
            });
            NewEmployee = new VmCommand(() =>
            {
                Employee = new();

            });
            NewDep = new VmCommand(() =>
            {
                DepartmentDTO dep = new DepartmentDTO { IdMainDep = Department.Id};
                Department.ChildDepartments.Add(dep);
                Department = dep;
                DepDirector = null;
            });
        }

        private bool CheckSubDepartment(DepartmentDTO depMain, DepartmentDTO department)
        {
            foreach (var dep in depMain.ChildDepartments)
            {
                if (dep.Id == department.Id)
                    return true;
            }
            return false;
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
            var searchRezult = allEmployees.Where(s => string.IsNullOrEmpty(search) ||
                        (s.FirstName.Contains(search) ||
                        s.LastName.Contains(search) ||
                        s.Patronymic.Contains(search) ||
                        s.Email.Contains(search) ||
                        s.Phone.Contains(search)));
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
                MainDepartments = new ObservableCollection<DepartmentDTO>(allDepartments.Where(s => s.IdMainDep == null));
                Departments = new ObservableCollection<DepartmentDTO>(allDepartments.Where(s => s.IdMainDep != 0));

                var result1 = await REST.Instance.client.GetAsync("Users/GetAllUsers");
                if (result1.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    var test = await result1.Content.ReadAsStringAsync();
                    allEmployees = await result1.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);

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

            }
            catch { }
            Loaded?.Invoke(this, null);
        }



        public void CreateExpanders()
        {
            stack1.Children.Clear();
            //Button button = ((Expander)sender) as Button;
            StackPanel expanderPanel = new StackPanel();
            stack1.Children.Add(expanderPanel);
            var header = "";
            var mheader = "";
            var muser = new UserDTO();
            var user = new UserDTO();
            var mdirector = new UserDTO();
            var director = new UserDTO();
            foreach (var maindep in MainDepartments)
            {
                //mdirector = allEmployees.FirstOrDefault(s => s.IdDepartment == maindep.Id);
                //if (maindep.IdDirectorNavigation != null)
                mdirector = maindep.Director;
                if (mdirector != null)
                    mheader = $"{maindep.Title} - {mdirector.FIO}";
                else
                    mheader = maindep.Title;
                var mdepExp = new Expander { Header = mheader, BorderThickness = new Thickness(2), Background = Brushes.White, BorderBrush = brush };
                Expanders.Add(mdepExp);
                mdepExp.PreviewMouseDown += ExpanderClick;
                mdepExp.Tag = maindep;
                StackPanel expanderPanel1 = new StackPanel();
                foreach (var dep in Departments)
                {
                    //director = allEmployees.FirstOrDefault(s => s.IdDepartment == dep.Id);
                    //if (dep.IdDirectorNavigation != null)
                    director = dep.Director;
                    if (director != null)
                        header = $"{dep.Title} - {director.FIO}";
                    else
                        header = dep.Title;
                    if (dep.IdMainDep == maindep.Id)
                    {
                        var depExp = new Expander { Margin = new Thickness(20, 0, 0, 0), Header = header, Background = Brushes.White };
                        Expanders.Add(depExp);
                        depExp.PreviewMouseDown += ExpanderClick;
                        depExp.Tag = dep;
                        var users = dep.Users.Where(s => s.IdRole == 3 && s.IsDeleted == false);
                        //if (users.Where(s => string.IsNullOrEmpty(search) ||
                        //(s.FirstName.Contains(search) ||
                        //s.LastName.Contains(search) ||
                        //s.Patronymic.Contains(search) ||
                        //s.Email.Contains(search) ||
                        //s.Phone.Contains(search))).Count() > 0)
                        //    depExp.IsExpanded = false;
                        var list = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = users };
                        list.SelectionChanged += UserSelected;
                        depExp.Content = list;
                        expanderPanel1.Children.Add(depExp);
                    }
                }
                var usersListBox = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = maindep.Users.Where(s => s.IdRole == 3 && s.IsDeleted == false) };
                usersListBox.SelectionChanged += UserSelected;
                var usersExpander = new Expander { Margin = new Thickness(0, 0, 0, 20), Header = "Сотрудники", Background = Brushes.White, Content = usersListBox };
                Expanders.Add(usersExpander);
                expanderPanel1.Children.Add(usersExpander);
                mdepExp.Content = expanderPanel1;


                expanderPanel.Children.Add(mdepExp);
            }
        }

        private void UserSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Employee = e.AddedItems[0] as UserDTO;

            }
        }

        private void ExpanderClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Department = ((Expander)sender).Tag as DepartmentDTO;
            Employee = allEmployees.FirstOrDefault(s => s.Id == Department.IdDirector && s.IdRole != 4);
            if (Employee == null)
                Employee = new UserDTO { LastName = "Директор не назначен" };
            DepDirector = allEmployees.FirstOrDefault(s => s.Id == Department.IdDirector);
        }
        StackPanel stack1;
        private Visibility transferVisible;

        internal void SetStackDepartment(StackPanel stack1)
        {
            this.stack1 = stack1;
        }
    }
}
