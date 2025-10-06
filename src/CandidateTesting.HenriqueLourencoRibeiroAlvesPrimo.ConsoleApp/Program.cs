using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Constants;

// Configurar a aplicação
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configurar serviços
var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddApplicationServices(configuration);

using var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Configurações
var provider = configuration["CdnSettings:Provider"];
var version = configuration["Version"];

logger.LogInformation("=== Conversor de Logs CDN ===");
logger.LogInformation("Versão: {Version}", version);
logger.LogInformation("Provider: {Provider}", provider);

// Caminhos dos arquivos
var inputFile = ApplicationConstants.InputFileName;
var outputFile = ApplicationConstants.OutputFileName;

// Verificar se arquivo de entrada existe
if (!File.Exists(inputFile))
{
    logger.LogError("Arquivo de entrada '{InputFile}' não encontrado!", inputFile);
    return;
}

// Se arquivo de saída existe, apagar
if (File.Exists(outputFile))
{
    File.Delete(outputFile);
    logger.LogInformation("Arquivo de saída anterior removido: {OutputFile}", outputFile);
}

logger.LogInformation("Lendo arquivo de entrada: {InputFile}", inputFile);
logger.LogInformation("Gerando arquivo de saída: {OutputFile}", outputFile);

try
{
    // Executar conversão
    var convertLogs = serviceProvider.GetRequiredService<IConvertLogs>();
    var request = new ConvertLogsRequest(inputFile, outputFile);

    var result = await convertLogs.RunAsync(request);

    // Mostrar resultados
    logger.LogInformation("Conversão concluída com sucesso!");
    logger.LogInformation("Estatísticas:");
    logger.LogInformation("   • Linhas lidas: {LinesRead}", result.LinesRead);
    logger.LogInformation("   • Linhas convertidas: {LinesConverted}", result.LinesConverted);
    logger.LogInformation("   • Linhas inválidas: {LinesInvalid}", result.LinesInvalid);
    logger.LogInformation("   • Sucesso: {Success}", result.Success ? "Sim" : "Não");
    logger.LogInformation("   • Arquivo gerado: {TargetPath}", result.TargetPath);

    if (File.Exists(outputFile))
    {
        logger.LogInformation("Conteúdo do arquivo de saída:");
        logger.LogInformation("{Separator}", new string('=', 50));
        var outputContent = await File.ReadAllTextAsync(outputFile);
        logger.LogInformation("{OutputContent}", outputContent);
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Erro durante a conversão: {Message}", ex.Message);
}
