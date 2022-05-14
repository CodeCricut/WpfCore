namespace WpfCore.ViewManagement;

[Serializable]
internal class ViewForViewModelNotFoundException : Exception
{
	public ViewForViewModelNotFoundException()
	{
	}

	public ViewForViewModelNotFoundException(string message) : base(message)
	{
	}

	public ViewForViewModelNotFoundException(string message, Exception innerException) : base(message, innerException)
	{
	}

	protected ViewForViewModelNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}