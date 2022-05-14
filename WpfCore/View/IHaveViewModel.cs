using WpfCore.ViewModel;

namespace WpfCore.View;

/// <summary>
/// Views which use viewmodels inheriting from <see cref="BaseViewModel"/> should use this interface.
/// </summary>
/// <typeparam name="TViewModel">The type of viewmodel this view uses.</typeparam>
public interface IHaveViewModel<TViewModel>
	where TViewModel : BaseViewModel
{
	void SetViewModel(TViewModel viewModel);
}
