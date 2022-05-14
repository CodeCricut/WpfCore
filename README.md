# WpfCore

WpfCore is a lightweight, opinionated package with commonly used WPF MVVM classes, such as ICommand wrappers, view management solutions, and an event aggregator for global messaging.

Features:

-   Easy to register services for `IServiceCollection`
-   `BaseViewModel` implementation of `INotifyPropertyChanged` for data binding
-   `DelegateCommand` and `AsyncDelegateCommand` implementation of `ICommand` for easily creating command handlers
-   `IViewManager` for managing the opening and closing of windows/dialogs without violating inversion of control
-   `IEventAggregator` for simplifying global messaging between view models and other components of your app

## Quick Start

### Services and DI Container

If you would like to make use of one of the services of the project such as `IViewManager`, you need to register the project to add all necessary services to the `IServiceCollection`.

This will typically look like this:

```cs
// App.xaml.cs
public partial class App : Application
{
    public IServiceProvider Services { get; init; }

    public App()
    {
        IServiceCollection services = new ServiceCollection();
        IConfiguration config = new ConfigurationBuilder().Build();

        // Register services to container
        services.AddWpfCore(config);

        Services = services.BuildServiceProvider();
    }
}
```

From there, any other service can choose to inject one of WpfCore's services.

```cs
class MyCustomViewModel : BaseViewModel {
    private readonly IViewManager _viewManager;

    // If this view model is registered with the DI container, you may inject WpfCore services
    public MyCustomViewModel(IViewManager viewManager){
        _viewManager = viewManager;
    }

    public void Close(){
        _viewManager.close(this);
    }
}
```

### Opening the startup window

To open the main/startup window when the app starts, you should override `Application.OnStartup` and use the `IViewManager` to open that window using the associated view model:

```cs
// App.xaml.cs
public partial class App : Application
{
    // ...

    protected override void OnStartup(StartupEventArgs e)
    {
        var viewManager = Services.GetRequiredService<IViewManager>();
        var mainWindowVm = new MainWindowViewModel();
        viewManager.Show(mainWindowVm);
    }
}
```

## `BaseViewModel`

This library rests upon you using the MVVM pattern for your app. Most windows (views) and custom controls should be associated with a view model.

View models should inherit from `BaseViewModel`, which is a sensible implementation of WPF's `INotifyPropertyChanged` interface
required for data binding.

To trigger the `INotifyPropertyChanged.PropertyChanged` event for a particular property, you can either call `Set` in the setter or explicitly call `RaisePropertyChanged` for another property.

A simple example would look like

```cs
public class MyFormControlViewModel : BaseViewModel {
    private string _username;
    public string Username
    {
        get => _username;
        set =>
        {
            // Automatically raises PropertyChanged event for the Username
            Set(ref _username, value);

            // Manually trigger updates to other properties
            RaisePropertyChanged(nameof(CanSubmitForm));
        }
    }

    // This is dependent on Username
    public bool CanSubmitForm => String.IsNullOrWhiteSpace(Username);
}

```

## `DelegateCommand` and `AsyncDelegateCommand`

These two classes provide a sensible implementation of WPF's built-in `ICommand` interface used for event handlers. They should be used in the view model or backing logic for the view you want to register handlers for.

Example:

```xml
<Button Command="{Binding ViewModel.LoadAsyncCommand}">
    Load async
</Button>
<Button Command="{Binding ViewModel.CloseCommand}">
    Close
</Button>
```

```cs
public class DelegateCommandExampleViewModel : BaseViewModel
{
    public AsyncDelegateCommand LoadAsyncCommand { get; }
    public DelegateCommand CloseCommand { get; }

    public DelegateCommandExample()
    {
        LoadAsyncCommand = new AsyncDelegateCommand(LoadAsync);
        CloseCommand = new DelegateCommand(Close);
    }


    // Commands accept an object argument, which will default to null if not provided
    public async Task LoadAsync(object? param = null)
    {
        // do some long running async task...
    }

    public void Close(object? param = null)
    {
        // close window...
    }
}
```

## `IHaveViewModel` and the `IViewManager`

To avoid view models referencing the WPF `Window` class directly, which would lead to two-way references, view models should instead interact with the `IViewManager` in order to open/close windows and dialogs.

> "But how does a view model open a specific window without referencing said window?"

All windows should implement the fluently named `IHaveViewModel<TViewModel>` interface, which associates windows with their view models and vice-versa.

The view manager then only requires a view model parameter, which it will then find the associated window for.

This is best illustrated with an example:

```cs
public partial class MainWindow : Window, IHaveViewModel<MainWindowViewModel>
{
    public MainWindowViewModel ViewModel { get; set; }
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

    public MainWindowViewModel(IViewManager viewManager)
    {
        _viewManager = viewManager;
    }

    public void CloseWindow(){
        // The view model may open/close the current window (or other windows) without having a reference to any WPF specific class
        _viewManager.Close(this);
    }
}
```

In order to open new windows, you just need to have a reference a view model for the window:

```cs
// Elsewhere in the app
var mainWindowVm = new MainWindowViewModel();
viewManager.ShowDialog(mainWindowVm);
```

## `IEventAggregator`

Oftentimes while using the MVVM pattern, view models will have to communicate with each other in some way. This can lead to extremely tightly-bound and fragile designs because view models need references to any other view models they want to communicate with. Not to mention, it is difficult to enable two-way communication between view models because of a classic catch22 situation:

```cs
class SettingsViewModel : BaseViewModel
{
    // ...
    public SettingsViewModel(MainWindowViewModel mainWindowVm){
        _mainWindowVm = mainWindowVm;
    }

    public void UpdateTheme(Theme updatedTheme){
        _mainWindowVm.Theme = updatedTheme;
    }

    public void DisableSettingsPanel(){
        // ...
    }
}

class MainWindowViewModel : BaseViewModel
{
    // ...
    public MainWindowViewModel(SettingsViewModel settingsVm){
        _settingsVm = settingsVm;
    }

    public void DisableSettingsPanel(){
        _settingsViewModel.DisableSettingsPanel();
    }
}
```

In this example, `SettingsViewModel` needs a reference to `MainWindowViewModel` in order to update the theme, but `MainWindowViewModel` needs a reference to `SettingsViewModel` in order to disable the settings panel.

Without doing some clever workarounds, it would be impossible to instantiate either view model as we need the other view model first!

### Using `IEventAggregator`

The `IEventAggregator` alleviates the issue of communication between classes. It will typically be used by view models.

It works like a message bus where you can register/unregister handlers for specific message types, and send/post those messages to have all handlers invoked.

In the situation described above, it would look like this:

```cs
class UpdateThemeMessage {
    public Theme NewTheme { get; }

    public UpdateThemeMessage(Theme newTheme){
        NewTheme = newTheme;
    }
}

class DisableSettingsPanelMessage {
    // If no parameters to send, no need for custom constructor
}

class SettingsViewModel : BaseViewModel
{
    // ...
    public SettingsViewModel(IEventAggregator ea){
        // No need to inject the other view model!
        _ea = ea;

    // When _ea.SendMessage(new DisableSettingsPanelMessage()) is called, this will handle that message
        _ea.RegisterHandler<DisableSettingsPanelMessage>(msg => DisableSettingsPanel());
    }

    public void UpdateTheme(Theme updatedTheme){
        _ea.SendMessage(new UpdateThemeMessage(updatedTheme));
    }

    public void DisableSettingsPanel(){
        // ...
    }
}

class MainWindowViewModel : BaseViewModel
{
    // ...
    public MainWindowViewModel(IEventAggregator ea){
        _ea = ea;

        // When _ea.SendMessage(new UpdateThemeMessage(updatedTheme)) is called, this will handle that message
        _ea.RegisterHandler<UpdateThemeMessage>(msg => Theme = msg.NewTheme);
    }

    public void DisableSettingsPanel(){
        _ea.SendMessage(new DisableSettingsPanelMessage());
    }
}
```

While this is a powerful tool, it can lead to hard-to-trace control flow if used too liberally. It is advised to only use this pattern when:

-   you need two-way communication between view models
-   you need to send a message to a different part of the app which wouldn't be easily referenced directly
-   you need to send a message to many handlers at once
