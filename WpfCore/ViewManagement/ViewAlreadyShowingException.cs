namespace WpfCore.ViewManagement;

[Serializable]
internal class ViewAlreadyShowingException : Exception
{
	public ViewAlreadyShowingException()
	{
	}

	public ViewAlreadyShowingException(string message) : base(message)
	{
	}

	public ViewAlreadyShowingException(string message, Exception innerException) : base(message, innerException)
	{
	}

	protected ViewAlreadyShowingException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}