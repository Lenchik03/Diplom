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
using System.Windows.Shapes;

namespace ProjectSystemWPF.View
{
    /// <summary>
    /// Логика взаимодействия для EditProjectPage.xaml
    /// </summary>
    public partial class EditProjectPage : Window
    {
        public EditProjectPage(ProjectDTO project)
        {
            InitializeComponent();
            if(project.Id != 0)
                (DataContext as EditProjectVM).GetProject(project);
            else
            {
                (DataContext as EditProjectVM).GetProject(new ProjectDTO());
            }
                (DataContext as EditProjectVM).Loaded += Vm_Loaded;
                (DataContext as EditProjectVM).SetWindow(this);
        }
        private void Vm_Loaded(object? sender, EventArgs e)
        {
            //var vm = DataContext as TransferUserVM;
            //vm.CreateExpanders();

        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((EditProjectVM)DataContext).Save?.Execute(null);
            }
        }
    }
}
