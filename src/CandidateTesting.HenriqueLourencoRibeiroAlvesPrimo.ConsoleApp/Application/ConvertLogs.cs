using System;
using System.Threading;
using System.Threading.Tasks;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Response;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Application;

public sealed class ConvertLogs : IConvertLogs
{
    private readonly ISourceReader _sourceReader;
    private readonly ILineParser<CdnLogEntry> _lineParser;
    private readonly IConvertLine<CdnLogEntry, CdnLogExit> _converter;
    private readonly ITargetWriter _writer;
    private readonly ILogger<ConvertLogs> _logger;

    public ConvertLogs(
        ISourceReader sourceReader,
        ILineParser<CdnLogEntry> lineParser,
        IConvertLine<CdnLogEntry, CdnLogExit> converter,
        ITargetWriter writer,
        ILogger<ConvertLogs> logger)
    {
        _sourceReader = sourceReader ?? throw new ArgumentNullException(nameof(sourceReader));
        _lineParser = lineParser ?? throw new ArgumentNullException(nameof(lineParser));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ConvertLogsResponse> RunAsync(
        ConvertLogsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SourceUrl))
            throw new ArgumentException("SourceUrl não pode ser vazio.", nameof(request.SourceUrl));

        if (string.IsNullOrWhiteSpace(request.TargetPath))
            throw new ArgumentException("TargetPath não pode ser vazio.", nameof(request.TargetPath));

        var startedAt = DateTime.Now;
        var linesRead = 0;
        var linesConverted = 0;
        var linesInvalid = 0;

        // cabeçalho do ficheiro de saída
        await _writer.WriteHeaderAsync(DateTimeOffset.Now, request.TargetPath, cancellationToken);

        await foreach (var line in _sourceReader.ReadLinesAsync(request.SourceUrl, cancellationToken))
        {
            linesRead++;
            if (string.IsNullOrWhiteSpace(line))
            {
                // linha vazia: ignora mas conta como lida
                continue;
            }

            if (!_lineParser.TryParse(line, out var entry))
            {
                linesInvalid++;
                _logger?.LogWarning("Linha inválida descartada: {Line}", line);
                continue;
            }

            var converted = _converter.Convert(entry!);
            await _writer.AppendAsync(converted, request.TargetPath, cancellationToken);
            linesConverted++;
        }

        var finishedAt = DateTime.Now;

        // monta o DTO de resposta
        var response = new ConvertLogsResponse(
            TargetPath: request.TargetPath,
            LinesRead: linesRead,
            LinesConverted: linesConverted,
            LinesInvalid: linesInvalid,
            StartTime: startedAt,
            FinnisthTime: finishedAt,
            Success: linesInvalid == 0
        );

        return response;
    }
}
