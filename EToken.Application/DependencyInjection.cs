using EToken.Application.Commands;
using EToken.Application.Commons.Interfaces;
using EToken.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EToken.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IClock, SystemClock>();

        services.AddScoped<ITotpService, TotpService>();

        services.AddScoped<AdminSeedHandler>();
        return services;
    }
}