using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
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
    /// Логика взаимодействия для ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public ProjectPage()
        {
            InitializeComponent();
        }

        private void EditProjectClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListBox;
            var  p = list.SelectedItem as ProjectDTO;
            if (p != null)
                ((ProjectVM)DataContext).Select(p);
        }
    }
}
