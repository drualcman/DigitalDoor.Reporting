namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.TryAddScoped<ReportsPresenter>();
        services.TryAddScoped<IReportsOutputPort>(serivce => serivce.GetService<ReportsPresenter>());
        services.TryAddScoped<IReportsPresenter>(serivce => serivce.GetService<ReportsPresenter>());
        return services;
    }
}
