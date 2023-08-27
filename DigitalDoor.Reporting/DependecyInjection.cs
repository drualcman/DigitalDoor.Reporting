using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Presenters;
using DigitalDoor.Reporting.Controllers;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.AddScoped<ReportsPresenter>();
        services.AddScoped<IReportsOutputPort>(serivce => serivce.GetService<ReportsPresenter>());
        services.AddScoped<IReportsPresenter>(serivce => serivce.GetService<ReportsPresenter>());

        services.AddScoped<PDFReportPresenter>();
        services.AddScoped<IPDFReportOutputPort>(service => service.GetService<PDFReportPresenter>());
        services.AddScoped<IPDFReportPresenter>(services => services.GetService<PDFReportPresenter>());

        services.AddScoped<IReportAsBytes, PDFReportController>();

        return services;
    }
}
