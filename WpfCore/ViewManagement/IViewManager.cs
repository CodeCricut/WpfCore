using WpfCore.ViewModel;

namespace WpfCore.ViewManagement;

/// <summary>
/// Manages the opening/closing of views for this application.
/// </summary>
public interface IViewManager
{
	/// <summary>
	/// Show the view associated with <paramref name="viewModel"/>.
	/// </summary>
	/// <param name="viewModel"></param>
	void Show<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;

	/// <summary>
	/// Open a dialog associated with <paramref name="viewModel"/>, blocking the thread until the dialog is closed.
	/// </summary>
	/// <param name="viewModel"></param>
	bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;

	/// <summary>
	/// Close the view associated with <paramref name="viewModel"/>. <see cref="Show(BaseViewModel)"/> must have been previously
	/// called with the same <paramref name="viewModel"/> reference.
	/// </summary>
	/// <param name="viewModel"></param>
	void Close(BaseViewModel viewModel);
}
