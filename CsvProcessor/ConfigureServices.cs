using CsvProcessor.Config;
using CsvProcessor.Models.DbModels;
using CsvProcessor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CsvProcessor;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var serverSettings = configuration.GetSection(nameof(ServerSettings));

        services.AddPooledDbContextFactory<CsvContext>(options =>
        {
           options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
               sqlOptions => { sqlOptions.EnableRetryOnFailure(); });
        });

        services.AddDbContextPool<CsvContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => { sqlOptions.EnableRetryOnFailure(); });
            options.EnableSensitiveDataLogging();
        });

        services.AddTransient<CsvProcessorService>();
        services.AddTransient<DbService>();

        return services;
    }
}