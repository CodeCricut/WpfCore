namespace WpfCore.Commands;

/// <summary>
/// Simple implementation of <see cref="ICommand"/> which executes a delegate action
/// when the command is executed.
/// </summary>
public class DelegateCommand : ICommand
{
	private readonly Action<object?>? _execute;
	private readonly Func<object?, bool>? _canExecute;

	public event EventHandler? CanExecuteChanged;

	public DelegateCommand(Action<object?>? execute, Func<object?, bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	public void RaiseCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool CanExecute(object? parameter = null)
	{
		return _canExecute == null || _canExecute(parameter);
	}

	public void Execute(object? parameter = null)
	{
		_execute?.Invoke(parameter);
	}
}
