using System.Windows.Input;

namespace ExampleProject;

public partial class ProfileWindow : Window, IHaveViewModel<ProfileWindowViewModel>
{
	public ProfileWindowViewModel ViewModel { get; private set; }
	public ProfileWindow()
	{
		InitializeComponent();
	}

	public void SetViewModel(ProfileWindowViewModel viewModel)
	{
		ViewModel = viewModel;
	}
}

public class ProfileWindowViewModel : BaseViewModel
{
	private readonly IViewManager _viewManager;

	public string Username { get; }
	public string Password { get; }

	public ICommand BackCommand { get; }

	public ProfileWindowViewModel(IViewManager viewManager, string username, string password)
	{
		_viewManager = viewManager;
		Username = username;
		Password = password;
		BackCommand = new DelegateCommand(_ => NavigateBack());
	}

	private void NavigateBack()
	{
		// Open the main window
		var mainWindowVm = new MainWindowViewModel(_viewManager);
		_viewManager.Show(mainWindowVm);

		// Close this window
		_viewManager.Close(this);
	}
}
