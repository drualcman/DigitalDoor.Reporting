namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.TryAddScoped<ReportsPresenter>();
        services.TryAddScoped<IReportsOutputPort>(serivce => serivce.GetService<ReportsPresenter>());
        services.TryAddScoped<IReportsPresenter>(serivce => serivce.GetService<ReportsPresenter>());

        services.TryAddScoped<PDFReportPresenter>();
        services.TryAddScoped<IPDFReportOutputPort>(service => service.GetService<PDFReportPresenter>());
        services.TryAddScoped<IPDFReportPresenter>(services => services.GetService<PDFReportPresenter>());

        services.TryAddScoped<IReportAsBytes, PDFReportController>();

        return services;
    }
}
