using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectSystemWPF.ViewModel
{
    public class CommandParameter<T> : ICommand
    {
        Action<T> action;

        public CommandParameter(Action<T> action)
        {
            this.action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCamExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action((T)parameter);
        }
    }
}
