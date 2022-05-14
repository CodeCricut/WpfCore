namespace WpfCore.EventAggregator;

/// <summary>
/// Functionality interface for an Event Aggregator.
/// </summary>
public interface IEventAggregator
{
	/// <summary>
	/// Send a message 
	/// </summary>
	/// <remarks>
	/// Sends the message and waits for processing on the UI thread before returning.
	/// </remarks>
	/// <typeparam name="T">Type of the message</typeparam>
	/// <param name="message">Message to send</param>
	void SendMessage<T>(T message);

	/// <summary>
	/// Post a message 
	/// </summary>
	/// Posts the message for later processing on the UI thread, returning immediately.
	/// <typeparam name="T">Type of the message</typeparam>
	/// <param name="message">Message to send</param>
	void PostMessage<T>(T message);

	/// <summary>
	/// Register a message handler
	/// </summary>
	/// <param name="eventHandler">Message handler to add.</param>
	/// <returns>Registered delegate</returns>
	void RegisterHandler<T>(Action<T> eventHandler);

	/// <summary>
	/// Unregister a message handler
	/// </summary>
	/// <param name="eventHandler">Message handler to remove.</param>
	void UnregisterHandler<T>(Action<T> eventHandler);
}
