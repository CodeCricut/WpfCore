using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleProject;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
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

    protected override void OnStartup(StartupEventArgs e)
    {
        var viewManager = Services.GetRequiredService<IViewManager>();
        var mainWindowVm = new MainWindowViewModel(viewManager);
        viewManager.Show(mainWindowVm);
    }
}
