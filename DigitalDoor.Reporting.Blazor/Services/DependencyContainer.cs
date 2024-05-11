namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddReportingBlazorServices(this IServiceCollection services)
    {
        services.AddReportingServices();
        services.AddScoped<GenerateReportAsBytes>();
        return services;
    }
}
