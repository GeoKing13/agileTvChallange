using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.In.Sources;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Processing;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Adapter.Mapping;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Out;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Application;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

// Configurar a aplicação
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configurar logging
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddSingleton<IConfiguration>(configuration);

// Configurar CdnSettings
var cdnSettings = new CdnSettings
{
    Provider = configuration["CdnSettings:Provider"] ?? "MINHA CDN"
};
services.AddSingleton(cdnSettings);

// Registrar serviços
services.AddSingleton<ISourceReader, FileSourceReader>();
services.AddSingleton<ILineParser<CdnLogEntry>, MinhaCdnLineParser>();
services.AddSingleton<IConvertLine<CdnLogEntry, CdnLogExit>, ConvertLog>();
services.AddSingleton<ITargetWriter, CdnOutFileWriter>();
services.AddSingleton<IConvertLogs, ConvertLogs>();

var serviceProvider = services.BuildServiceProvider();

// Configurações
var provider = configuration["CdnSettings:Provider"];
var version = configuration["Version"];

Console.WriteLine("=== Conversor de Logs CDN ===");
Console.WriteLine($"Versão: {version}");
Console.WriteLine($"Provider: {provider}");
Console.WriteLine();

// Caminhos dos arquivos
var inputFile = "input.log";
var outputFile = "output.log";

// Verificar se arquivo de entrada existe
if (!File.Exists(inputFile))
{
    Console.WriteLine($"❌ Arquivo de entrada '{inputFile}' não encontrado!");
    return;
}

// Se arquivo de saída existe, apagar
if (File.Exists(outputFile))
{
    File.Delete(outputFile);
    Console.WriteLine($"🗑️ Arquivo de saída anterior removido: {outputFile}");
}

Console.WriteLine($"📖 Lendo arquivo de entrada: {inputFile}");
Console.WriteLine($"📝 Gerando arquivo de saída: {outputFile}");
Console.WriteLine();

try
{
    // Executar conversão
    var convertLogs = serviceProvider.GetRequiredService<IConvertLogs>();
    var request = new ConvertLogsRequest(inputFile, outputFile);

    var result = await convertLogs.RunAsync(request);

    // Mostrar resultados
    Console.WriteLine("✅ Conversão concluída com sucesso!");
    Console.WriteLine($"📊 Estatísticas:");
    Console.WriteLine($"   • Linhas lidas: {result.LinesRead}");
    Console.WriteLine($"   • Linhas convertidas: {result.LinesConverted}");
    Console.WriteLine($"   • Linhas inválidas: {result.LinesInvalid}");
    Console.WriteLine($"   • Sucesso: {(result.Success ? "✅ Sim" : "❌ Não")}");
    Console.WriteLine($"   • Arquivo gerado: {result.TargetPath}");

    if (File.Exists(outputFile))
    {
        Console.WriteLine($"\n📄 Conteúdo do arquivo de saída:");
        Console.WriteLine(new string('=', 50));
        var outputContent = await File.ReadAllTextAsync(outputFile);
        Console.WriteLine(outputContent);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro durante a conversão: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
