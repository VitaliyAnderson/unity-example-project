using System;
using System.Windows.Input;

public class DelegateCommand : ICommand
{
    private readonly Func<object, bool> _canExecute;
    private readonly Action<object> _execute;

    public event EventHandler CanExecuteChanged;

    public DelegateCommand(Action<object> execute)
    {
        if (execute is null)
        {
            throw new ArgumentNullException("execute");
        }

        _execute = execute;
    }

    public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
    {
        if (canExecute is null)
        {
            throw new ArgumentNullException("canExecute");
        }

        if (execute is null)
        {
            throw new ArgumentNullException("execute");
        }

        _canExecute = canExecute;
        _execute = execute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute is null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
