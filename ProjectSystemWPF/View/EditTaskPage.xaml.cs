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
    /// Логика взаимодействия для EditTaskPage.xaml
    /// </summary>
    public partial class EditTaskPage : Window
    {
        public EditTaskPage(TaskDTO task)
        {
            InitializeComponent();
            if (task.Id != 0)
                (DataContext as EditTaskPage).GetTask(task);
            else
            {
                (DataContext as EditTaskPage).GetTask(new TaskDTO());
            }
                (DataContext as EditTaskPage).Loaded += Vm_Loaded;
        }

        

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            //var vm = DataContext as TransferUserVM;
            //vm.CreateExpanders();

        }
    }
    }
}
