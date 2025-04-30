using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            (DataContext as ChatsVM).SetDispatcher(Dispatcher);
                 ((INotifyCollectionChanged)listBox.Items).CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }

        private void EditChatClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ListBox;
            var p = list.SelectedItem as ChatDTO;
            if (p != null)
                ((ChatsVM)DataContext).Select(p);
        }

       

        private void EditMessageClick(object sender, RoutedEventArgs e)
        {
            if(listBox.SelectedItem != null)
            {
                var message = listBox.SelectedItem as MessageDTO;
                if(message.IdSender == ActiveUser.GetInstance().User.Id)
                {
                    ((ChatsVM)DataContext).EditMessage(message);
                }
            }
        }

        private void DeleteMessageClick(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                var message = listBox.SelectedItem as MessageDTO;
                if (message.IdSender == ActiveUser.GetInstance().User.Id)
                {
                    ((ChatsVM)DataContext).DeleteMessageAsync(message);
                }
            }
        }
    }
}
