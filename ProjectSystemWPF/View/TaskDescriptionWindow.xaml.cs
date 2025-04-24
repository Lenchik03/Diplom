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
    /// Логика взаимодействия для TaskDescriptionWindow.xaml
    /// </summary>
    public partial class TaskDescriptionWindow : Window
    {
        public TaskDescriptionWindow(TaskDTO task)
        {
            InitializeComponent();
            if (task.Id != 0)
            {
                (DataContext as TaskDescriptionVM).SetTask(task);
            }
            else
            {
                (DataContext as TaskDescriptionVM).SetTask(new TaskDTO());
            }
            
        }
    }
}
