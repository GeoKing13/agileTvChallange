using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Response;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Application;

public sealed class ConvertLogs : IConvertLogs
{
    private readonly ISourceReader _sourceReader;

    public async Task<ConvertLogsResponse> RunAsync(ConvertLogsRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}