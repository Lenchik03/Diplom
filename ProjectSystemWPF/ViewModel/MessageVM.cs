using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using MaterialDesignColors;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public bool CanEditChat
        { get => canEditChat;
            set { canEditChat = value;
                Signal();
            } }
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
        public ChatDTO Chat { get => chat1;
            set
            {
                chat1 = value;
                
                Signal();
            }
        }
        public VmCommand NewChat { get; set; }
        public Visibility EditVisible
        {
            get => editVisible;
            set { editVisible = value;
                Signal();
            }
        }



        public VmCommand SelectImage { get; set; }
        ObservableCollection<DepartmentDTO> allDepartments = new();
        ObservableCollection<UserDTO> allEmployees = new();

        private List<UserDTO> selectedUser = new();


        public event EventHandler Loaded;

        public MessageVM()
        {
            GetLists();
            

            SelectImage = new VmCommand(async () =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    var fileName = Path.GetFileName(filePath);
                    var fileContent = await File.ReadAllBytesAsync(filePath);
                    Chat.ImagePath = fileContent;
                    Chat.ImageSourse = filePath;
                    Signal(nameof(Chat.ImageSourse));
                    Signal(nameof(Chat));
                }
            });
            NewChat = new VmCommand(async () =>
            {
                if (Chat.Id == 0)
                {
                    Chat.IdCreator = ActiveUser.GetInstance().User.Id;
                    Chat.Creator = ActiveUser.GetInstance().User;
                    Chat.IsDeleted = false;
                    string arg = JsonSerializer.Serialize(Chat, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Chats",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        Chat = await responce.Content.ReadFromJsonAsync<ChatDTO>(REST.Instance.options);
                        Signal(nameof(Chat));

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
                        //MessageBox.Show("Отдел был добавлен!");
                        
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Произошла ошибка. Заполните все данные!");
                        return;
                    }

                    
                }
                else
                {
                    string arg = JsonSerializer.Serialize(Chat, REST.Instance.options);
                    var responce = await REST.Instance.client.PutAsync($"Chats/{Chat.Id}",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        
                        selectedUser.Add(ActiveUser.GetInstance().User);
                        string arg1 = JsonSerializer.Serialize(selectedUser, REST.Instance.options);
                        var responce1 = await REST.Instance.client.PostAsync($"Chats/AddNewMembers/{Chat.Id}",
                            new StringContent(arg1, Encoding.UTF8, "application/json"));
                        try
                        {
                            responce.EnsureSuccessStatusCode();
                            MessageBox.Show("Чат успешно обновлен!");
                            //MessageBox.Show("Отдел был добавлен!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Произошла ошибка при добавлении участников!");
                            return;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка");
                        return;
                    }

                    
                }
                newMessageWindow.Close();
                GetLists();
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

       
        private ChatDTO chat;
        private int countPart;

        public Control CreateTreeView(NewMessageWindow newMessageWindow)
        {
            var treeview = new TreeView();

            var controlStyle = newMessageWindow.FindResource("treeView") as Style;

            //TreeViewItem mtreeviewitem;
            foreach (var maindep in MainDepartments)
            {
                TreeViewItem mtreeviewitem = new TreeViewItem { DataContext = maindep, Style = controlStyle, Background = Brushes.LightYellow };
                mtreeviewitem.Tag = maindep;
                mtreeviewitem.Header = maindep.Title;
                var t = maindep.Users.Where(s => s.IsDeleted == false).ToList();
                if (t.Count != 0)
                {
                    foreach (var emp in maindep.Users.Where(s => s.IdRole != 4 && s.Id != 59 && s.IsDeleted == false))
                    {
                        if (Chat != null && Chat.ChatUsers.FirstOrDefault(s=>s.IdUser == emp.Id) != null)
                            emp.Selected = true;
                        if (emp.Id == maindep.IdDirector)
                            mtreeviewitem.Items.Add(new TreeViewItem { DataContext = emp, Style = controlStyle, Tag = emp, Header = emp.FIO });
                    }
                }
                if (maindep.ChildDepartments.Count != 0)
                {
                    foreach (var dep in maindep.ChildDepartments)
                    {
                        if (dep.Users.Where(s => s.IsDeleted == false).ToList().Count != 0)
                        {
                            TreeViewItem treeViewItem = new TreeViewItem { DataContext = dep, Style = controlStyle };
                            treeViewItem.Tag = dep;
                            treeViewItem.Header = dep.Title;
                            foreach (var emps in dep.Users.Where(s => s.IdRole != 4 && s.Id != 59 && s.IsDeleted == false))
                            {
                                if (Chat != null && Chat.ChatUsers.FirstOrDefault(s => s.IdUser == emps.Id) != null)
                                    emps.Selected = true;
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
                    if (maindep.Users.Where(s => s.IsDeleted == false).ToList().Count != 0)
                    {
                        foreach (var emp in maindep.Users.Where(s => s.IdRole != 4 && s.Id != 59 && s.IsDeleted == false))
                        {
                            if (Chat != null && Chat.ChatUsers.FirstOrDefault(s => s.IdUser == emp.Id) != null)
                                emp.Selected = true;
                            if (emp.Id != maindep.IdDirector)
                                mtreeviewitem.Items.Add(new TreeViewItem { DataContext = emp, Style = controlStyle, Tag = emp, Header = emp.FIO });
                        }
                    }
                }
                treeview.Items.Add(mtreeviewitem);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = treeview;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            return scrollViewer;
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
        private ChatDTO chat1 = new();
        private bool canEditChat = true;
        private Visibility editVisible = Visibility.Visible;

        internal void SetWindow(NewMessageWindow newMessageWindow)
        {
            this.newMessageWindow = newMessageWindow;
        }

        internal void GetChat(ChatDTO chat)
        {
            Chat = chat;
            if (Chat.IdCreator != null)
            {
                EditVisible = Chat.IdCreator == ActiveUser.GetInstance().User.Id ? Visibility.Visible : Visibility.Collapsed;
                CanEditChat = Chat.IdCreator == ActiveUser.GetInstance().User.Id ? true : false;
                
            }
                
            Loaded?.Invoke(this, null);
        }
    }
}
