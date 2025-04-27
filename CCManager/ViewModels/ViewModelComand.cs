using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCManager.ViewModels
{
    public class ViewModelComand : ICommand
    {
        //Archivos
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecuteAction;
        private Func<object, bool> canExecuteLoginCommand;

        //Constructor
        public ViewModelComand(Action<object> executeAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = null;
        }

        public ViewModelComand(Action<object> executeAction, Func<object, bool> canExecuteLoginCommand) : this(executeAction)
        {
            this.canExecuteLoginCommand = canExecuteLoginCommand;
        }


        //Eventos
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //Metodos
        public bool CanExecute(object parameter)
        {
            return _canExecuteAction == null || _canExecuteAction(parameter);

        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
