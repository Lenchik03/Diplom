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

namespace ProjectSystemWPF.View
{
    /// <summary>
    /// Логика взаимодействия для TransferUserWindow.xaml
    /// </summary>
    public partial class TransferUserWindow : Window
    {
        public TransferUserWindow(UserDTO user)
        {
            InitializeComponent();
            (DataContext as TransferUserVM).GetUser(user);
            (DataContext as TransferUserVM).GetWindow(this);
            (DataContext as TransferUserVM).Loaded += Vm_Loaded;
            /*
            var vm = new TransferUserVM();
            vm.Loaded += Vm_Loaded;
            if (user != null)
            {
                vm.GetUser(user);
            }
            DataContext = vm;
            */
        }
        private void Vm_Loaded(object? sender, EventArgs e)
        {
            //var vm = DataContext as TransferUserVM;
            //vm.CreateExpanders();
            
        }


    }
}
