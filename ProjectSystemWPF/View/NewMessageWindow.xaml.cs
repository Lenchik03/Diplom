using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using ProjectSystemWPF.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectSystemWPF
{
    /// <summary>
    /// Логика взаимодействия для NewMessageWindow.xaml
    /// </summary>
    public partial class NewMessageWindow : Window
    {
        public NewMessageWindow(ChatDTO chat)
        {
            InitializeComponent();
            (DataContext as MessageVM).Loaded += Vm_Loaded;
            (DataContext as MessageVM).SetWindow(this);
            if (chat.Id != 0)
                (DataContext as MessageVM).GetChat(chat);
            else
            {
                (DataContext as MessageVM).GetChat(new ChatDTO());
            }
            
        }

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            stackpanel.Children.Clear();
            var stackpanel1 = (DataContext as MessageVM).CreateTreeView(this);
            stackpanel.Children.Add(stackpanel1);
        }

        private void CheckedDepartment(object sender, RoutedEventArgs e)
        {
            SetChecked(sender, true);
        }

        private void UncheckedDepartment(object sender, RoutedEventArgs e)
        {
            SetChecked(sender, false);
        }

        private void SetChecked(object sender, bool result)
        {
            var tag = ((CheckBox)sender).DataContext;
            var selectedUser = new List<UserDTO>();
            List<UserDTO> users;
            if (tag is DepartmentDTO dep)
            {               
                foreach (var depChild in dep.ChildDepartments)
                {
                    depChild.Selected = result;
                    foreach (var user in depChild.Users)
                    {
                        user.Selected = result;
                        selectedUser.Add(user);
                    }
                }
                foreach (var user in dep.Users)
                {
                    user.Selected = result;
                    selectedUser.Add(user);
                }
                (DataContext as MessageVM).DoThingsAsync(selectedUser, result);
            }
            else if (tag is UserDTO user)
                (DataContext as MessageVM).DoThingsAsync(new List<UserDTO>{ user}, result);
        }

    }
}
