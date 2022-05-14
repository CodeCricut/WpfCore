using System.Windows.Input;

namespace ExampleProject;

public partial class MainWindow : Window, IHaveViewModel<MainWindowViewModel>
{
    public MainWindowViewModel ViewModel { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
    }

    public void SetViewModel(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;
    }
}

public class MainWindowViewModel : BaseViewModel
{
    private readonly IViewManager _viewManager;

	private string _username;
	public string Username
	{
		get => _username;
		set {
            Set(ref _username, value);
            SubmitCommand.RaiseCanExecuteChanged();
        }
	}

	private string _password;
	public string Password
	{
        get => _password;
		set {
            Set(ref _password, value);
            SubmitCommand.RaiseCanExecuteChanged();
        }
    }

	public bool CanSubmitForm => !(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)); 

    public DelegateCommand SubmitCommand { get; }

    public DelegateCommand CloseCommand { get; }

    public MainWindowViewModel(IViewManager viewManager)
    {
        _viewManager = viewManager;

        SubmitCommand = new DelegateCommand(_ => Submit(), _ => CanSubmitForm);
        CloseCommand = new DelegateCommand(_ => CloseWindow());
    }

    public void Submit()
	{
        // Open the profile window
        var profileWindowVm = new ProfileWindowViewModel(_viewManager, Username, Password);
        _viewManager.Show(profileWindowVm);

        // Close this window
        _viewManager.Close(this);
	}

    public void CloseWindow()
    {
        // The view model may open/close the current window (or other windows) without having a reference to any WPF specific class
        _viewManager.Close(this);
    }
}
