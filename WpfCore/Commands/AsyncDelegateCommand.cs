namespace WpfCore.Commands;

/// <summary>
/// Simple implementation of <see cref="ICommand"/> which executes an asynchronous delegate action
/// when the command is executed.
/// </summary>
public class AsyncDelegateCommand : ICommand
{
	private readonly Func<object?, Task>? _execute;
	private readonly Func<object?, bool>? _canExecute;

	public event EventHandler? CanExecuteChanged;

	public AsyncDelegateCommand(Func<object?, Task>? execute, Func<object?, bool>? canExecute = null)
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

	public async void Execute(object? parameter = null)
	{
		if (_execute != null)
		{
			await _execute.Invoke(parameter);
		}
	}
}
