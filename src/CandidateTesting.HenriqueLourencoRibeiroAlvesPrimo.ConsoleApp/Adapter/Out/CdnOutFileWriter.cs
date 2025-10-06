using System.Globalization;
using System.Text;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Out;

public sealed class CdnOutFileWriter : ITargetWriter
{
    private readonly string _version;

    public CdnOutFileWriter(IConfiguration configuration)
    {
        _version = configuration["Version"] ?? "1.0";
    }

    private string[] HeaderLines =>
        new[]
        {
            $"#Version: {_version}",
            "#Fields: provider http-method status-code uri-path time-taken response-size cache-status"
        };

    public async Task AppendAsync(CdnLogExit entry, string targetPath, CancellationToken ct = default)
    {
        var line = FormatarLinha(entry);
        await File.AppendAllTextAsync(targetPath, line + Environment.NewLine, Encoding.UTF8, ct);
    }

    public async Task WriteHeaderAsync(DateTimeOffset date, string targetPath, CancellationToken ct = default)
    {
        var directory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var header = new StringBuilder();
        header.AppendLine(HeaderLines[0]);
        header.AppendLine($"#Date: {date:dd/MM/yyyy HH:mm:ss}");
        header.AppendLine(HeaderLines[1]);

        await File.WriteAllTextAsync(targetPath, header.ToString(), Encoding.UTF8, ct);
    }

    private static string FormatarLinha(CdnLogExit e)
    {
        return string.Create(CultureInfo.InvariantCulture,
            $"\"{e.Provider}\" {e.HttpMethod} {e.StatusCode} {e.UriPath} {e.TimeTaken} {e.ResponseSize} {e.CacheStatus}");
    }
}
