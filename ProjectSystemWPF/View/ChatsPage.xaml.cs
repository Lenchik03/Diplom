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
    /// Логика взаимодействия для ChatsPage.xaml
    /// </summary>
    public partial class ChatsPage : Page
    {
        public ChatsPage()
        {
            InitializeComponent();
        }

        private void EditChatClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListBox;
            var p = list.SelectedItem as ChatDTO;
            if (p != null)
                ((ChatsVM)DataContext).Select(p);
        }
    }
}
