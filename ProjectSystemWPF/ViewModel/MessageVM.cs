using MaterialDesignColors;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace ProjectSystemWPF.ViewModel
{
    public class MessageVM: BaseVM
    {
        private ObservableCollection<UserDTO> employees;
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
        public event EventHandler Loaded;


        public MessageVM()
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

        ObservableCollection<Node> nodes;
        public StackPanel CreateTreeView(NewMessageWindow newMessageWindow)
        {
            StackPanel panel = new StackPanel();
            var header = "";
            var mheader = "";
            var muser = new UserDTO();
            var user = new UserDTO();
            var mdirector = new UserDTO();
            var director = new UserDTO();
            var treeview = new TreeView();

            var controlStyle = newMessageWindow.FindResource("treeView") as Style;

            //TreeViewItem mtreeviewitem;
            foreach (var maindep in MainDepartments)
            {
                TreeViewItem mtreeviewitem = new TreeViewItem { DataContext = maindep, Style = controlStyle, Background = Brushes.LightYellow};
                mtreeviewitem.Tag = maindep;
                mtreeviewitem.Header = maindep.Title;
                if (maindep.ChildDepartments.Count != 0)
                {
                    foreach (var dep in maindep.ChildDepartments)
                    {
                        if (dep.Users.Count != 0)
                        {
                            TreeViewItem treeViewItem = new TreeViewItem{ DataContext = dep, Style = controlStyle };
                            treeViewItem.Tag = dep;
                            treeViewItem.Header = dep.Title;
                            foreach(var emps in dep.Users)
                            {
                                treeViewItem.Items.Add(new TreeViewItem { DataContext = emps, Style = controlStyle, Tag = emps, Header = emps.FIO });
                            }
                            mtreeviewitem.Items.Add(treeViewItem);
                        }
                        else
                        {
                            mtreeviewitem.Items.Add(new TreeViewItem { DataContext = dep, Style = controlStyle, Tag = dep, Header = dep.Title });
                        }

                    }

                }
                else
                {
                    if(maindep.Users.Count != 0)
                    {
                        foreach(var emp in maindep.Users)
                        {
                            if (emp.Id != maindep.IdDirector)
                                mtreeviewitem.Items.Add(new TreeViewItem { DataContext = emp, Style = controlStyle, Tag = emp, Header = emp.FIO });
                        }
                    }
                }
                treeview.Items.Add(mtreeviewitem);
            }

            panel.Children.Add(treeview);
            return panel;


                

                


                //var mdepExp = new Expander { Header = maindep.Title, BorderThickness = new Thickness(2), Background = Brushes.White, BorderBrush = brush };
                //mdepExp.Tag = maindep;
                //StackPanel expanderPanel1 = new StackPanel();
                //foreach (var dep in Departments)
                //{
                //    director = allEmployees.FirstOrDefault(s => s.IdDepartment == dep.Id);
                //    //if (dep.IdDirectorNavigation != null)
                //    if (director != null)
                //        header = $"{dep.Title} - {director.FIO}";
                //    else
                //        header = dep.Title;
                //    if (dep.IdMainDep == maindep.Id)
                //    {
                //        var depExp = new Expander { Margin = new Thickness(20, 0, 0, 0), Header = header, Background = Brushes.White };
                //        depExp.PreviewMouseDown += ExpanderClick;
                //        depExp.Tag = dep;
                //        var list = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = dep.Users.Where(s => s.IdRole == 3) };
                //        list.SelectionChanged += UserSelected;
                //        depExp.Content = list;
                //        expanderPanel1.Children.Add(depExp);
                //    }
                //}
                //var usersListBox = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = maindep.Users.Where(s => s.IdRole == 3) };
                //usersListBox.SelectionChanged += UserSelected;
                //var usersExpander = new Expander { Margin = new Thickness(0, 0, 0, 20), Header = "Сотрудники", Background = Brushes.White, Content = usersListBox };
                //expanderPanel1.Children.Add(usersExpander);
                //mdepExp.Content = expanderPanel1;


                //expanderPanel.Children.Add(mdepExp);
            //}
            //return expanderPanel;
        }

        internal void DoThings(List<UserDTO> selectedUser)
        {
            throw new NotImplementedException();
        }
    }
}
