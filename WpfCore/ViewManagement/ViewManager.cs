using WpfCore.View;
using WpfCore.ViewModel;

namespace WpfCore.ViewManagement;

/// <summary>
/// Manages the opening/closing of windows for this WPF application.
/// </summary>
class ViewManager : IViewManager
{
	private readonly Dictionary<BaseViewModel, Window> _activeViews = new();
	private readonly IViewFinder _viewFinder;

	public ViewManager(IViewFinder viewFinder)
	{
		_viewFinder = viewFinder;
	}

	public void Show<TViewModel>(TViewModel viewModel)
		where TViewModel : BaseViewModel
	{
		AssertAssociatedViewNotActive(viewModel);

		Window view = _viewFinder.FindViewForViewModel(viewModel);

		Application.Current.Dispatcher.Invoke(() =>
		{
			((IHaveViewModel<TViewModel>)view).SetViewModel(viewModel);
			view.Show();

			_activeViews.Add(viewModel, view);
		});
	}

	public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
	{
		AssertAssociatedViewNotActive(viewModel);

		Window view = _viewFinder.FindViewForViewModel(viewModel);

		return Application.Current.Dispatcher.Invoke(() =>
			{
				((IHaveViewModel<TViewModel>)view).SetViewModel(viewModel);
				_activeViews.Add(viewModel, view);

				return view.ShowDialog();
			});
	}

	public void Close(BaseViewModel viewModel)
	{
		Application.Current.Dispatcher.Invoke(() =>
		{
			if (!_activeViews.ContainsKey(viewModel)) return false;

			var view = _activeViews.GetValueOrDefault(viewModel);

			view?.Close();

			return _activeViews.Remove(viewModel);
		});
	}

	private void AssertAssociatedViewNotActive<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel
	{
		if (_activeViews.ContainsKey(viewModel))
			throw new ViewAlreadyShowingException($"View for {viewModel} view model already showing. Ensure that the view is closed before opening it again.");
	}
}
