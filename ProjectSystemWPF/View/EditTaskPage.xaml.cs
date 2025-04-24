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
            (DataContext as EditTaskVM).Loaded += Vm_Loaded;
            if (task.Id != 0)
                (DataContext as EditTaskVM).SetTask(task);
            else
            {
                (DataContext as EditTaskVM).SetTask(new TaskDTO());
            }
            (DataContext as EditTaskVM).SetWindow(this);
        }

        

        private void Vm_Loaded(object? sender, EventArgs e)
        {
            //var vm = DataContext as TransferUserVM;
            //vm.CreateExpanders();

        }
    }
    
}
