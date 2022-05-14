using Scrutor;
using WpfCore.EventAggregator;
using WpfCore.ViewManagement;

namespace WpfCore;

public static class DependencyInjection
{
	/// <summary>
	/// Add the core components needed for a WPF application to the <paramref name="services"/>
	/// using the provided <paramref name="configuration"/>.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static IServiceCollection AddWpfCore(this IServiceCollection services, IConfiguration configuration)
	{
		if (services == null) throw new ArgumentNullException(nameof(services));
		if (configuration == null) throw new ArgumentNullException(nameof(configuration));

		services.RegisterAllWindows();

		services.AddSingleton<IEventAggregator, EventAggregator.EventAggregator>();

		services.RegisterViewFinder()
		  .AddSingleton<IViewManager, ViewManager>();

		return services;
	}

	private static IServiceCollection RegisterViewFinder(this IServiceCollection services)
	{
		return services.AddSingleton<IViewFinder>(sp =>
			{
				var viewFinder = new ViewFinder(sp);
				return viewFinder;
			});
	}

	private static IServiceCollection RegisterAllWindows(this IServiceCollection services)
	{
		return services.Scan(scan =>
						scan.FromEntryAssembly()
							.AddClasses(c => c.AssignableTo<Window>())
							.UsingRegistrationStrategy(RegistrationStrategy.Skip)
							.AsSelf()
							.WithTransientLifetime());
	}
}
