using MaterialDesignColors.Recommended;
using ProjectSystemWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectSystemWPF.ViewModel
{
    public class SuperUserVM: BaseVM
    {
        public ObservableCollection<Department> MainDepartments { get; set; } = new();
        public ObservableCollection<Department> Departments { get; set; } = new();
        public ObservableCollection<Role> Roles { get; set; } = new();
        public ObservableCollection<User> Employees
        {
            get => employees;
            set { employees = value; Signal(); }
            
        }
        ObservableCollection<Department> allDepartments = new();
        ObservableCollection<User> allEmployees = new();
        public bool CanEdit
        {
            get => canEdit;
            set { canEdit = value;
                Signal();
            }
        }
        public VmCommand NewEmployee { get; set; }
        public User Employee
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
        public VmCommand CanEditClick { get; set; }
        public bool CanEditDep 
        { 
            get => canEditDep;
            set { canEditDep = value;
                Signal();
            }
        }
        public VmCommand NewDep { get; set; }
        public Department Department
        {
            get => department;
            set
            { 
                department = value;
                Signal();
            }
        }
        public ObservableCollection<User> Directors 
        { get => directors; set 
            { 
                directors = value;
                Signal();
            } }
        public User DepDirector
        {
            get => depDirector;
            set { depDirector = value; Signal(); }
        }
        public VmCommand SaveDep { get; set; }
        public VmCommand CanEditDepClick { get; set; }
        public Button SelectedDepOrUser { get; set; }

        Brush brush = new SolidColorBrush(Color.FromArgb(255, 223, 196, 01));
        private User employee = new User
        {
            Password = "",
            IdRoleNavigation = new Role { Id = 3, Title = "" },
            IdDepartmentNavigation = new Department { Id = 1, Title = "" }
        };
        private Department department;
        private ObservableCollection<User> employees;
        private User depDirector;
        private ObservableCollection<User> directors = new();
        private bool canEdit;
        private bool canEditDep;

        public event EventHandler Loaded;

        public SuperUserVM()
        {
            GetLists();

            if (ActiveUser.GetInstance().User.IdRole == 3)
            {
                Hidden = Visibility.Collapsed;
            }
            else
                Hidden = Visibility.Visible;

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
                if(Employee != null && Employee.Id !=0)
                {
                    
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
                    Employee = new User();
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
                            Employee.IdDepartmentNavigation = Department;
                            Employee.IdRole = 1;
                            Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 1);
                            Department.IdDirector = Employee.Id;
                            Department.IdDirectorNavigation = Employee;
                        }
                        
                    }
                    if (ActiveUser.GetInstance().User.IdRole == 1)
                    {
                        if (ActiveUser.GetInstance().User.IdDepartmentNavigation.InverseIdMainDepNavigation.Count > 0)
                        {
                            Employee.IdRole = 3;
                            Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 3);
                            Employee.IdDepartment = ActiveUser.GetInstance().User.IdDepartment;
                            Employee.IdDepartmentNavigation = ActiveUser.GetInstance().User.IdDepartmentNavigation;
                        }
                        else
                        {
                            Employee.IdRole = 2;
                            Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 2);
                            Employee.IdDepartment = Department.Id;
                            Employee.IdDepartmentNavigation = Department;
                        }
                    }
                    if (ActiveUser.GetInstance().User.IdRole == 2)
                    {
                        Employee.IdRole = 3;
                        Employee.IdRoleNavigation = Roles.FirstOrDefault(s => s.Id == 3);
                        Employee.IdDepartment = ActiveUser.GetInstance().User.IdDepartment;
                        Employee.IdDepartmentNavigation = ActiveUser.GetInstance().User.IdDepartmentNavigation;
                    }
                    Employee.Password = "";
                    string arg = JsonSerializer.Serialize(Employee, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Users/AddNewUser",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    //MessageBox.Show(responce.StatusCode.ToString());
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Всё ок");

                        string arg1 = JsonSerializer.Serialize(Department, REST.Instance.options);
                        var responce1 = await REST.Instance.client.PutAsync($"Departments",
                            new StringContent(arg, Encoding.UTF8, "application/json"));
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
                    catch (Exception ex)
                    {
                        MessageBox.Show("Всё не ок " + ex.Message);
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
            var result = await REST.Instance.client.GetAsync("Departments");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                allDepartments = await result.Content.ReadFromJsonAsync<ObservableCollection<Department>>(REST.Instance.options);
            }
            MainDepartments = new ObservableCollection<Department>(allDepartments.Where(s => s.IdMainDep == null));
            Departments = new ObservableCollection<Department>(allDepartments.Where(s => s.IdMainDep != 0));

            var result1 = await REST.Instance.client.GetAsync("Users/GetAllUsers");
            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                var test = await result1.Content.ReadAsStringAsync();
                allEmployees = await result1.Content.ReadFromJsonAsync<ObservableCollection<User>>(REST.Instance.options);
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



        public StackPanel CreateExpanders()
        {
            //Button button = ((Expander)sender) as Button;
            StackPanel expanderPanel = new StackPanel();
            var header = "";
            var mheader = "";
            var muser = new User();
            var user = new User();
            foreach (var maindep in MainDepartments)
            {
                if (maindep.IdDirectorNavigation != null)
                    mheader = $"{maindep.Title} - {maindep.IdDirectorNavigation.FIO}";
                else
                    mheader = maindep.Title;
                var mdepExp = new Expander { Header = mheader, BorderThickness = new Thickness(2), Background = Brushes.White, BorderBrush = brush };
                mdepExp.PreviewMouseDown += ExpanderClick;
                mdepExp.Tag = maindep;
                StackPanel expanderPanel1 = new StackPanel();
                foreach (var dep in Departments)
                {
                    if (dep.IdDirectorNavigation != null)
                        header = $"{dep.Title} - {dep.IdDirectorNavigation.FIO}";
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
                Employee = e.AddedItems[0] as User;

            }
        }

        private void ExpanderClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Department = ((Expander)sender).Tag as Department;
            Employee = allEmployees.FirstOrDefault(s => s.Id == Department.IdDirector);
            Directors = new ObservableCollection<User>(allEmployees.Where(s => s.IdDepartment == Department.Id));
            DepDirector = allEmployees.FirstOrDefault(s => s.Id == Department.IdDirector);
        }
    }
}
