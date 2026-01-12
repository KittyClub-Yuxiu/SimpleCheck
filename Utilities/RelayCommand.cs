using System;
using System.Windows.Input;

namespace SimpleCheck.Utilities
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            // 检查参数是否可以转换为 T 类型
            T? typedParameter = default;
            if (parameter is T t) {
                typedParameter = t;
            }
            return _canExecute == null || (typedParameter != null && _canExecute(typedParameter));
        }

        public void Execute(object? parameter)
        {
            // 检查参数是否可以转换为 T 类型
            T? typedParameter = default;
            if (parameter is T t) {
                typedParameter = t;
            }
            if (typedParameter != null || default(T) == null)
                _execute(typedParameter);
            else
                throw new ArgumentNullException(nameof(parameter));
        }
    }
}