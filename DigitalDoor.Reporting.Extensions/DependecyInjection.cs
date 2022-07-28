﻿using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Presenters;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalDoor.Reporting.Extensions;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.AddScoped<ReportsPresenter>();
        services.AddScoped<IReportsOutputPort>(serivce => serivce.GetService<ReportsPresenter>());
        services.AddScoped<IReportsPresenter>(serivce => serivce.GetService<ReportsPresenter>());

        return services;
    }
}
