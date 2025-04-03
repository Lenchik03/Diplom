using MaterialDesignColors.Recommended;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class SuperUserVM: BaseVM
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
        ObservableCollection<UserDTO> allEmployees = new();
        public bool CanEdit
        {
            get => canEdit;
            set { canEdit = value;
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
        public Visibility Hidden { get; set; }
        public Visibility HiddenEditUser { get; set; }
        public Visibility HiddenEditDep { get; set; }
        public VmCommand CanEditClick { get; set; }
        public bool CanEditDep 
        { 
            get => canEditDep;
            set { canEditDep = value;
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
                Signal();
            }
        }
        public ObservableCollection<UserDTO> Directors 
        { get => directors; set 
            { 
                directors = value;
                Signal();
            } }
        public UserDTO DepDirector
        {
            get => depDirector;
            set { depDirector = value; Signal(); }
        }
        public VmCommand SaveDep { get; set; }
        public VmCommand CanEditDepClick { get; set; }
        public Button SelectedDepOrUser { get; set; }

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

        public event EventHandler Loaded;

        public SuperUserVM()
        {
            GetLists();
            var dep = allDepartments.FirstOrDefault(s => s.Id == ActiveUser.GetInstance().User.IdDepartment);
            if (ActiveUser.GetInstance().User.IdRole == 3)
            {
                Hidden = Visibility.Collapsed;
                HiddenEditUser = Visibility.Collapsed;
            }
            else
            {
                Hidden = Visibility.Visible;
                
                foreach(var emp in allEmployees)
                {
                    if (emp.IdDepartment == dep.Id)
                    {
                        HiddenEditUser = Visibility.Visible;
                    }
                        
                    else
                    {
                        HiddenEditUser = Visibility.Collapsed;
                    }
                        
                }
            }
            if(ActiveUser.GetInstance().User.IdRole == 3 || ActiveUser.GetInstance().User.IdRole == 2)
            {
                HiddenEditDep = Visibility.Collapsed;
            }
            if(ActiveUser.GetInstance().User.IdRole == 2 && dep == Department)
            {
                HiddenEditDep = Visibility.Visible;
            }
            else
            {
                Hidden = Visibility.Collapsed;
            }
            if (ActiveUser.GetInstance().User.IdRole == 1 && dep == Department)
            {
                HiddenEditDep = Visibility.Visible;
            }
            else
            {
                Hidden = Visibility.Collapsed;
            }
            if (ActiveUser.GetInstance().User.IdRole == 4)
            {
                HiddenEditDep = Visibility.Visible;
                HiddenEditUser = Visibility.Visible;
            }


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
                if (Employee != null && Employee.Id !=0)
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
                        if(Department == null)
                        {
                            MessageBox.Show("Выберите отдел, куда добавить сотрудника!");
                            return;
                        }
                        else
                        {
                            Employee.IdDepartment = Department.Id;
                            //Employee.IdDepartmentNavigation = Department;
                            Employee.IdRole = 1;
                            //Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 1);
                            Department.IdDirector = Employee.Id;
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
                            Employee.IdRole = 2;
                            //Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 2);
                            Employee.IdDepartment = Department.Id;
                            //Employee.IdDepartmentNavigation = Department;
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

                    string arg = JsonSerializer.Serialize(Employee, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Users/AddNewUser",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    //MessageBox.Show(responce.StatusCode.ToString());
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Сотрудник был добавлен");
                        MailAddress fromAdress = new MailAddress("nikitina@suz-ppk.ru", ActiveUser.GetInstance().User.FIO);
                        MailAddress toAdress = new MailAddress(Employee.Email);
                        MailMessage message = new MailMessage(fromAdress, toAdress);
                        message.Body = "Добрый день, " + Employee.FIO + "! " + Environment.NewLine + "Ваш новый логин: " + Employee.Email + " " + Environment.NewLine + "Ваш новый пароль: " + Employee.Password + " ";
                        message.Subject = "Регистрация нового пользователя";

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.Host = "smtp.beget.com";
                        smtpClient.Port = 25;
                        //smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(fromAdress.Address, "zzPwr%j0");

                        smtpClient.Send(message);

                        if (Employee.IdRole == 1 || Employee.IdRole == 2)
                        {
                            //var str = await responce.Content.ReadAsStringAsync();
                            var answerUser = await responce.Content.ReadFromJsonAsync<UserDTO>(REST.Instance.options);

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
                        MessageBox.Show("Произошла ошибка. Заполните все данные!");
                        return;
                    }

                    
                }

                GetLists();
            });
            SaveDep = new VmCommand(async () =>
            {              
                if (Department.Id != 0 && Department != null)
                {
                    string arg = JsonSerializer.Serialize(Department, REST.Instance.options);
                    var responce = await REST.Instance.client.PutAsync($"Departments",
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
            });
            NewEmployee = new VmCommand(() =>
            {
                Employee = new();

            });
            NewDep = new VmCommand(() =>
            {
                Department = new();
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

            }
            catch { }
            Loaded?.Invoke(this, null);
        }



        public StackPanel CreateExpanders()
        {
            //Button button = ((Expander)sender) as Button;
            StackPanel expanderPanel = new StackPanel();
            var header = "";
            var mheader = "";
            var muser = new UserDTO();
            var user = new UserDTO();
            var mdirector = new UserDTO();
            var director = new UserDTO();
            foreach (var maindep in MainDepartments)
            {
                mdirector = allEmployees.FirstOrDefault(s => s.IdDepartment == maindep.Id);
                //if (maindep.IdDirectorNavigation != null)
                if (mdirector != null)
                    mheader = $"{maindep.Title} - {mdirector.FIO}";
                else
                    mheader = maindep.Title;
                var mdepExp = new Expander { Header = mheader, BorderThickness = new Thickness(2), Background = Brushes.White, BorderBrush = brush };
                mdepExp.PreviewMouseDown += ExpanderClick;
                mdepExp.Tag = maindep;
                StackPanel expanderPanel1 = new StackPanel();
                foreach (var dep in Departments)
                {
                    director = allEmployees.FirstOrDefault(s => s.IdDepartment == dep.Id);
                    //if (dep.IdDirectorNavigation != null)
                    if (director != null)
                        header = $"{dep.Title} - {director.FIO}";
                    else
                        header = dep.Title;
                    if (dep.IdMainDep == maindep.Id)
                    {
                        var depExp = new Expander { Margin = new Thickness(20, 0, 0, 0), Header = header, Background = Brushes.White };
                        depExp.PreviewMouseDown += ExpanderClick;
                        depExp.Tag = dep;
                        var list = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = dep.Users.Where(s => s.IdRole == 3) };
                        list.SelectionChanged += UserSelected;
                        depExp.Content = list;
                        expanderPanel1.Children.Add(depExp);
                    }
                }
                var usersListBox = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = maindep.Users.Where(s => s.IdRole == 3) };
                usersListBox.SelectionChanged += UserSelected;
                var usersExpander = new Expander { Margin = new Thickness(0, 0, 0, 20), Header = "Сотрудники", Background = Brushes.White, Content = usersListBox };
                expanderPanel1.Children.Add(usersExpander);
                mdepExp.Content = expanderPanel1;


                expanderPanel.Children.Add(mdepExp);
            }
            return expanderPanel;
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
            Directors = new ObservableCollection<UserDTO>(allEmployees.Where(s => s.IdDepartment == Department.Id && s.IdRole != 4));
            DepDirector = allEmployees.FirstOrDefault(s => s.Id == Department.IdDirector);
        }
    }
}
