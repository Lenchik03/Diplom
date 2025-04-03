using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class ActiveUser
    {
        public UserDTO User
        {
            get => user;
            set
            {
                user = value;
                UserChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler UserChanged;
        private static ActiveUser instance;
        private UserDTO user;

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
