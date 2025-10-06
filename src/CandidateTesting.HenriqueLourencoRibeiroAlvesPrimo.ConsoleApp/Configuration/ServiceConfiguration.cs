using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.In.Sources;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Processing;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Adapter.Mapping;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Out;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Application;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Constants;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Configurar CdnSettings
        var cdnSettings = new CdnSettings
        {
            Provider = configuration["CdnSettings:Provider"] ?? ApplicationConstants.DefaultProvider
        };
        services.AddSingleton(cdnSettings);

        // Registrar serviços da aplicação
        services.AddSingleton<ISourceReader, FileSourceReader>();
        services.AddSingleton<ILineParser<CdnLogEntry>, MinhaCdnLineParser>();
        services.AddSingleton<IConvertLine<CdnLogEntry, CdnLogExit>, ConvertLog>();
        services.AddSingleton<ITargetWriter, CdnOutFileWriter>();
        services.AddSingleton<IConvertLogs, ConvertLogs>();

        return services;
    }
}
