using MaterialDesignColors.Recommended;
using ProjectSystemWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectSystemWPF.ViewModel
{
    public class SuperUserVM
    {
        public ObservableCollection<Department> MainDepartments { get; set; } = new();
        public ObservableCollection<Department> Departments { get; set; } = new();
        public ObservableCollection<User> Employees { get; set; }
        ObservableCollection<Department> allDepartments = new();
        ObservableCollection<User> allEmployees = new();
        public bool CanEdit { get; set; } = true;
        public VmCommand NewEmployee { get; set; }
        public User Employee { get; set; }
        public VmCommand SaveUser { get; set; }
        public Visibility Hidden { get; set; }
        public VmCommand CanEditClick { get; set; }
        public bool CanDelete { get; set; }
        public VmCommand NewDep { get; set; }
        public Department Department { get; set; }
        public ObservableCollection<User> Directors { get; set; } = new();
        public User DepDirector { get; set; }
        public VmCommand SaveDep { get; set; }
        public VmCommand CanEditDepClick { get; set; }
        public Button SelectedDepOrUser { get; set; }

        Brush brush = new SolidColorBrush(Color.FromArgb(255, 223, 196, 01));

        public event EventHandler Loaded;

        public SuperUserVM()
        {
            GetLists();
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

            var result1 = await REST.Instance.client.GetAsync("Users");
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                allEmployees = await result1.Content.ReadFromJsonAsync<ObservableCollection<User>>(REST.Instance.options);
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
                muser = maindep.Users.FirstOrDefault(s => s.Id == maindep.IdDirector);

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

        }
    }
}
