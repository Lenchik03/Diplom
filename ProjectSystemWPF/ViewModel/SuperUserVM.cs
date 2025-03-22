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

        public event EventHandler Loaded;

        public SuperUserVM()
        {
            GetLists();
        }
        public async void GetLists()
        {
            var result = await REST.Instance.client.GetAsync("Departments");
            //todo not ok

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                allDepartments = await result.Content.ReadFromJsonAsync<ObservableCollection<Department>>(REST.Instance.options);
            }
            MainDepartments = new ObservableCollection<Department>( allDepartments.Where(s=>s.IdMainDep==null));
            Departments = new ObservableCollection<Department>( allDepartments.Where(s=>s.IdMainDep!=0));

            var result1 = await REST.Instance.client.GetAsync("Users");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                allEmployees = await result1.Content.ReadFromJsonAsync<ObservableCollection<User>>(REST.Instance.options);
            }

            Loaded?.Invoke(this, null);
        }

        public StackPanel CreateExpanders() 
        {
            StackPanel expanderPanel = new StackPanel();
            var header = "";
            var mheader = "";
            var muser = new User();
            var user = new User();
            foreach (var maindep in MainDepartments) 
            {
                //var mdepExp = new Expander { Header = maindep.Title };
                //if(maindep.Users.Count != 0)
                //{
                //    mdepExp.Content = new ListBox { ItemsSource = maindep.Users }; 

                //}
                //else
                //{
                //    StackPanel expanderPanel1 = new StackPanel();
                //    foreach(var dep in Departments)
                //    {
                //        if(dep.IdMainDep == maindep.Id)
                //        {
                //            var depExp = new Expander { Header = dep.Title };
                //            depExp.Content = new ListBox { ItemsSource = dep.Users };
                //            expanderPanel1.Children.Add(depExp);
                //        }                     
                //    }
                //    mdepExp.Content = expanderPanel1;
                //}
                muser = maindep.Users.FirstOrDefault(s => s.Id == maindep.IdDirector);
                
                if (muser != null)
                    mheader = $"{maindep.Title}{muser.FIO}";
                else
                    mheader = maindep.Title;
                var mdepExp = new Expander { Header = mheader, BorderThickness = new Thickness(2), Background=Brushes.White, BorderBrush = Brushes.Yellow };
              
                    StackPanel expanderPanel1 = new StackPanel();
                    foreach (var dep in Departments)
                    {
                        user = dep.Users.FirstOrDefault(s => s.Id == dep.IdDirector);
                    
                    if (user != null)
                        header = $"{dep.Title}{user.FIO}";
                    else
                         header = dep.Title;
                        if (dep.IdMainDep == maindep.Id)
                        {
                            var depExp = new Expander { Margin = new Thickness(20, 0, 0, 0), Header = header, Background = Brushes.White};
                            depExp.Content = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = dep.Users.Where(s => s.IdRole == 3) };
                            expanderPanel1.Children.Add(depExp);
                        }
                    }
                expanderPanel1.Children.Add(new Expander { Margin=new Thickness(0,0,0,20), Header = "Сотрудники", Background = Brushes.White, Content = new ListBox { Margin = new Thickness(40, 0, 0, 0), ItemsSource = maindep.Users.Where(s => s.IdRole == 3) } }); 
                    mdepExp.Content = expanderPanel1;
                

                expanderPanel.Children.Add(mdepExp);
            }
            return expanderPanel;
        }
        
        
    }
}
