using ProjectSystemWPF.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSystemWPF.ViewModel
{
    public class MainVM: BaseVM
    {
        private Page currentPage;

        public static MainVM Instance { get; set; }


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
        //public VmCommand SignOut { get; set; }
        //public VmCommand MainPage { get; set; }

        public MainVM()
        {
            Instance = this;
            CurrentPage = new LoginPage();
            //Search = new VmCommand(() =>
            //{
            //    OpenSearch();
            //});
            //CurrentPage = new DirectorPage();

            //SignOut = new VmCommand(() =>
            //{
            //    CurrentPage = new LoginPage();
            //});

            //MainPage = new VmCommand(() =>
            //{
            //    CurrentPage = new DirectorPage();
            //});



            
        }

        //private void OpenSearch()
        //{

        //    CurrentPage = new CustomerPage(this);
        //    //CurrentPage = new ExecutorPage(this);
        //}
    }
}
