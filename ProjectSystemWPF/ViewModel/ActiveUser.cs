using ProjectSystemWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class ActiveUser
    {
        public User User { get; set; }
        private static ActiveUser instance;

        private ActiveUser()
        { }

        public static ActiveUser GetInstance()
        {
            if (instance == null)
                instance = new ActiveUser();
            return instance;
        }
    }
}
