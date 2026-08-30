using EToken.Application.Commons.Interfaces;
using EToken.Application.Interfaces;
using EToken.Infrastructure.Persistence.Repositories;
using EToken.Infrastructure.Security;
using EToken.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EToken.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerDeviceRepository, CustomerDeviceRepository>();
        services.AddScoped<ICustomerDeviceService, CustomerDeviceService>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ITokenService, TokenService>();
 services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<JwtService>();
 services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
                services.AddSingleton<ISecretStore, AesGcmSecretStore>();


        return services;
    }
}