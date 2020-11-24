using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Input;

namespace mpp_3.ViewModels.Base
{
    public class CommandBase : ICommand
    {
        public CommandBase(Action _action)
        {
            action = _action;
        }
        private Action action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            action();
        }
    }
}
