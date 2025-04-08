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
        public NewMessageWindow()
        {
            InitializeComponent();
            (DataContext as MessageVM).Loaded += Vm_Loaded;
        }

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            stackpanel.Children.Clear();
            var stackpanel1 = (DataContext as MessageVM).CreateTreeView(this);
            stackpanel.Children.Add(stackpanel1);
        }

        private void CheckedDepartment(object sender, RoutedEventArgs e)
        {

        }

        private void UncheckedDepartment(object sender, RoutedEventArgs e)
        {

        }
    }
}
