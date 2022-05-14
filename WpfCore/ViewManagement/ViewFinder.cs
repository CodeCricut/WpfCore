using WpfCore.View;
using WpfCore.ViewModel;

namespace WpfCore.ViewManagement;

/// <summary>
/// Interface for finding views associated with view models.
/// </summary>
interface IViewFinder
{
	/// <summary>
	/// Find the associated <see cref="Window"/> for the <paramref name="viewModel"/>.
	/// </summary>
	Window FindViewForViewModel<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;
}

class ViewFinder : IViewFinder
{
	private readonly IServiceProvider _viewServiceProvider;

	// Using the service provider directly is an antipattern, but we can use it just here to request views of a specific
	// type when those types aren't known at compile-time.
	public ViewFinder(IServiceProvider viewServiceProvider)
	{
		_viewServiceProvider = viewServiceProvider;
	}

	/// <summary>
	/// Find the associated <see cref="Window"/> for the <paramref name="viewModel"/> by searching the assembly
	/// for windows implementing <see cref="IHaveViewModel{TViewModel}"/> where <c>TViewModel</c> is the <typeparamref name="TViewModel"/> generic
	/// argument type. 
	/// </summary>
	public Window FindViewForViewModel<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
	{
		Type viewType = GetAssociatedViewType(viewModel.GetType());
		if (viewType == null) throw new ViewForViewModelNotFoundException($"Could not find a view associated with {viewModel}.");

		bool viewIsWindow = viewType.IsSubclassOf(typeof(Window)) || viewType == typeof(Window);
		if (!viewIsWindow) throw new AssociatedViewNotWindowException(
		   $"The view associated with {viewModel} was not a Window.");

		return (Window)_viewServiceProvider.GetRequiredService(viewType);
	}

	private Type GetAssociatedViewType(Type viewModelType)
	{
		IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes());

		Type viewForVM = types
			.FirstOrDefault(viewType => viewType
				.GetInterfaces()
					.Any(i =>
						i.IsGenericType &&
						i.GetGenericTypeDefinition() == typeof(IHaveViewModel<>) &&
						i.GenericTypeArguments.Contains(viewModelType)));

		return viewForVM;
	}
}
