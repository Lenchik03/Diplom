using ProjectSystemAPI.DB;
using ProjectSystemWPF.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectSystemWPF.ViewModel
{
    public class MainVM: BaseVM
    {
        private Page currentPage;
        public Visibility ViewMenu
        {
            get => viewMenu;
            set { viewMenu = value;
                Signal();
            }
        }
        private Visibility viewSignOut = Visibility.Collapsed;
        public Visibility MyProjectVisibility 
        { get => myProjectVisibility;
            set { myProjectVisibility = value;
                Signal();
            }
        }
        private Visibility viewMenu;
        private Visibility myProjectVisibility;

        public static MainVM Instance { get; set; }

        public Visibility ViewSignOut { 
            get => viewSignOut;
            set
            {
                viewSignOut = value;
                Signal();
            }
        }

        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                if (currentPage != null)
                {
                    if ((currentPage as Page).DataContext is IDisposable disposable)
                        disposable.Dispose();
                }
                currentPage = value;
                Signal();
            }
        }

        //public VmCommand Search { get; set; }
        public VmCommand SignOut { get; set; }
        public VmCommand OpenEmployeesClick { get; set; }
        public VmCommand OpenProfileClick { get; set; }
        public VmCommand OpenChatsClick { get; set; }
        public VmCommand OpenProjectClick { get; set; }
        public VmCommand OpenMyProjectClick { get; set; }
        //public VmCommand MainPage { get; set; }

        public MainVM()
        {
            Instance = this;
            ActiveUser.GetInstance().UserChanged += MainVM_UserChanged;
           
            CurrentPage = new LoginPage();
            //Search = new VmCommand(() =>
            //{
            //    OpenSearch();
            //});
            //CurrentPage = new DirectorPage();

            SignOut = new VmCommand(() =>
            {
                ActiveUser.GetInstance().User = null;
                CurrentPage = new LoginPage();
            });

            OpenProjectClick = new VmCommand(async () =>
            {
                CurrentPage = new ProjectPage();
            });

            OpenEmployeesClick = new VmCommand(() =>
            {
                CurrentPage = new SuperUserPage();
            });
            OpenProfileClick = new VmCommand(async () =>
            {
                CurrentPage = new NewUserPage();
            });

            OpenChatsClick = new VmCommand(async () =>
            {
                CurrentPage = new ChatsPage();
            });

            

            OpenMyProjectClick = new VmCommand(async () =>
            {
                CurrentPage = new MyProjectPage();
            });
            //MainPage = new VmCommand(() =>
            //{
            //    CurrentPage = new DirectorPage();
            //});




        }

        private void MainVM_UserChanged(object? sender, EventArgs e)
        {
            ViewSignOut = ActiveUser.GetInstance().User == null ? Visibility.Collapsed : Visibility.Visible;
            ViewMenu = ActiveUser.GetInstance().User == null ? Visibility.Collapsed : Visibility.Visible;
            if (ActiveUser.GetInstance().User != null)
            {
                if (ActiveUser.GetInstance().User.IdRole == 4)
                {
                    ViewMenu = Visibility.Collapsed;
                }
                if (ActiveUser.GetInstance().User.IdRole == 2 || ActiveUser.GetInstance().User.IdRole == 3)
                {
                    MyProjectVisibility = Visibility.Visible;
                }
                else
                {
                    MyProjectVisibility = Visibility.Collapsed;
                }
            }
        }

        //private void OpenSearch()
        //{

        //    CurrentPage = new CustomerPage(this);
        //    //CurrentPage = new ExecutorPage(this);
        //}
    }
}
