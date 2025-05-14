using ChatServerDTO.DTO;
using ProjectSystemWPF.ViewModel;
using System;
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
    /// Логика взаимодействия для MyProjectPage.xaml
    /// </summary>
    public partial class MyProjectPage : Page
    {
        public MyProjectPage()
        {
            InitializeComponent();
        }

        private void EditProjectClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListBox;
            var p = list.SelectedItem as ProjectDTO;
            if (p != null)
                ((MyProjectVM)DataContext).SelectAsync(p);
        }

        private void EditTaskClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListBox;
            var t = list.SelectedItem as TaskDTO;
            if (t != null)
                ((MyProjectVM)DataContext).SelectTask(t);
        }
    }
}
