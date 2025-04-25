using ProjectSystemWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ProjectSystemWPF.View
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            var vm = new LoginVM();  
            DataContext = vm;
            vm.SetPasswordBox(passwrdBox);
            Loaded += (s, e) => SignalR.Instance.CreateConnection();

        }

        private void MakeLol(object sender, RoutedEventArgs e)
        {
            ((Storyboard)FindResource("Lol")).Begin();
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MakeLol(sender, null);
                ((LoginVM)DataContext).OpenPage?.Execute(null);
            }
        }
    }
}
