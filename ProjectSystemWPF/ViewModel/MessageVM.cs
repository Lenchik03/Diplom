using MaterialDesignColors;
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
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace ProjectSystemWPF.ViewModel
{
    public class MessageVM : BaseVM
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
        public int CountPart
        {
            get => countPart;
            set
            {
                countPart = value;
                Signal();
            }
        }
        public Chat Chat { get; set; } = new();

        public VmCommand NewChat { get; set; }
        ObservableCollection<DepartmentDTO> allDepartments = new();
        ObservableCollection<UserDTO> allEmployees = new();

        private List<UserDTO> selectedUser = new();


        public event EventHandler Loaded;

        public MessageVM()
        {
            GetLists();

            NewChat = new VmCommand(async () =>
            {

                string arg = JsonSerializer.Serialize(Chat, REST.Instance.options);
                var responce = await REST.Instance.client.PostAsync($"Chats",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    Chat = await responce.Content.ReadFromJsonAsync<Chat>(REST.Instance.options);
                    //MessageBox.Show("Отдел был добавлен!");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Произошла ошибка. Заполните все данные!");
                    return;
                }

                selectedUser.Add(ActiveUser.GetInstance().User);
                string arg1 = JsonSerializer.Serialize(selectedUser, REST.Instance.options);
                var responce1 = await REST.Instance.client.PostAsync($"Chats/AddNewMembers/{Chat.Id}",
                    new StringContent(arg1, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    //MessageBox.Show("Отдел был добавлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при добавлении участников!");
                    return;
                }
                newMessageWindow.Close();
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

       
        private Chat chat;
        private int countPart;

        public StackPanel CreateTreeView(NewMessageWindow newMessageWindow)
        {
            StackPanel panel = new StackPanel();
            var treeview = new TreeView();

            var controlStyle = newMessageWindow.FindResource("treeView") as Style;

            //TreeViewItem mtreeviewitem;
            foreach (var maindep in MainDepartments)
            {
                TreeViewItem mtreeviewitem = new TreeViewItem { DataContext = maindep, Style = controlStyle, Background = Brushes.LightYellow };
                mtreeviewitem.Tag = maindep;
                mtreeviewitem.Header = maindep.Title;
                if (maindep.Users.Count != 0)
                {
                    foreach (var emp in maindep.Users.Where(s => s.IdRole != 4))
                    {
                        if (emp.Id != maindep.IdDirector)
                            mtreeviewitem.Items.Add(new TreeViewItem { DataContext = emp, Style = controlStyle, Tag = emp, Header = emp.FIO });
                    }
                }
                if (maindep.ChildDepartments.Count != 0)
                {
                    foreach (var dep in maindep.ChildDepartments)
                    {
                        if (dep.Users.Count != 0)
                        {
                            TreeViewItem treeViewItem = new TreeViewItem { DataContext = dep, Style = controlStyle };
                            treeViewItem.Tag = dep;
                            treeViewItem.Header = dep.Title;
                            foreach (var emps in dep.Users.Where(s => s.IdRole != 4))
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
                    if (maindep.Users.Count != 0)
                    {
                        foreach (var emp in maindep.Users.Where(s => s.IdRole != 4))
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
        }

        internal async System.Threading.Tasks.Task DoThingsAsync(List<UserDTO> selectedUser, bool result)
        {
            if (result)
                this.selectedUser = this.selectedUser.Union(selectedUser).ToList();
            else
                this.selectedUser = this.selectedUser.Except(selectedUser).ToList();
            CountPart = this.selectedUser.Count;
        }
        NewMessageWindow newMessageWindow;
        internal void SetWindow(NewMessageWindow newMessageWindow)
        {
            this.newMessageWindow = newMessageWindow;
        }
    }
}
