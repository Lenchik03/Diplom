using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectSystemWPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Unloaded += 
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        bool menuOpened = false;
        private void CLickMenu(object sender, RoutedEventArgs e)
        {
            Storyboard animation = null;
            if (!menuOpened)
            {
                animation = (Storyboard)FindResource("MenuOpen");
            }
            else
            {
                animation = (Storyboard)FindResource("MenuClose");
            }
            animation.Begin();
            menuOpened = !menuOpened;
        }
    }
}