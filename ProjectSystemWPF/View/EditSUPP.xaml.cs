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
    /// Логика взаимодействия для EditSUPP.xaml
    /// </summary>
    public partial class EditSUPP : Window
    {
        public EditSUPP(ProjectDTO project)
        {
            InitializeComponent();
            if (project.Id != 0)
                (DataContext as EditSUPVM).GetProject(project);

            (DataContext as EditSUPVM).Loaded += Vm_Loaded;
            (DataContext as EditSUPVM).SetWindow(this);
        }

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            
        }
    }
}
