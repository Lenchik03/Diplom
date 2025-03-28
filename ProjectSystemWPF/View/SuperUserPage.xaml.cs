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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectSystemWPF.View
{
    /// <summary>
    /// Логика взаимодействия для SuperUserPage.xaml
    /// </summary>
    public partial class SuperUserPage : Page
    {
        public SuperUserPage()
        {
            InitializeComponent();
            var vm = new SuperUserVM();
            vm.Loaded += Vm_Loaded;
            DataContext = vm;
        }

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            stack1.Children.Clear();
            var stackpanel = (DataContext as SuperUserVM).CreateExpanders();
            stack1.Children.Add(stackpanel);
        }
    }
}
