namespace WpfCore.EventAggregator;

class EventAggregator : IEventAggregator
{
	private readonly List<Delegate> _handlers = new();

	private readonly SynchronizationContext? _synchronizationContext;

	public EventAggregator()
	{
		_synchronizationContext = SynchronizationContext.Current;
	}

	public void SendMessage<T>(T message)
	{
		if (message == null) return;

		if (_synchronizationContext != null)
		{
			_synchronizationContext.Send(
				m => Dispatch((T)m),
				message);
		}
		else
		{
			Dispatch(message);
		}
	}

	public void PostMessage<T>(T message)
	{
		if (message == null) return;

		if (_synchronizationContext != null)
		{
			_synchronizationContext.Post(
				m => Dispatch((T)m),
				message);
		}
		else
		{
			Dispatch(message);
		}
	}

	public void RegisterHandler<T>(Action<T> eventHandler)
	{
		if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

		_handlers.Add(eventHandler);
	}

	public void UnregisterHandler<T>(Action<T> eventHandler)
	{
		if (eventHandler == null)
		{
			throw new ArgumentNullException(nameof(eventHandler));
		}

		_handlers.Remove(eventHandler);
	}

	/// <summary>
	/// Dispatch a message to all appropriate handlers
	/// </summary>
	/// <typeparam name="T">Type of the message</typeparam>
	/// <param name="message">Message to dispatch to registered handlers</param>
	private void Dispatch<T>(T message)
	{
		if (message == null)
		{
			throw new ArgumentNullException(nameof(message));
		}

		var compatibleHandlers
			= _handlers.OfType<Action<T>>()
				.ToList();
		foreach (var h in compatibleHandlers)
		{
			h(message);
		}
	}
}
