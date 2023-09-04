namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingPdfServices(this IServiceCollection services)
    {
        services.AddReportingServices();
        return services;
    }
}
