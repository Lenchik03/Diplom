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
        private Visibility viewSignOut = Visibility.Collapsed;

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
                currentPage = value;
                Signal();
            }
        }

        //public VmCommand Search { get; set; }
        public VmCommand SignOut { get; set; }
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

            //MainPage = new VmCommand(() =>
            //{
            //    CurrentPage = new DirectorPage();
            //});




        }

        private void MainVM_UserChanged(object? sender, EventArgs e)
        {
            ViewSignOut = ActiveUser.GetInstance().User == null ? Visibility.Collapsed : Visibility.Visible;
        }

        //private void OpenSearch()
        //{

        //    CurrentPage = new CustomerPage(this);
        //    //CurrentPage = new ExecutorPage(this);
        //}
    }
}
