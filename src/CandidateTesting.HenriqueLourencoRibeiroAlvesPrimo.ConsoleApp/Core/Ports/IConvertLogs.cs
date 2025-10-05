using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Response;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

/// <summary>
/// Execute the log streamming from sourceUrl to targetPath
/// </summary>
public interface IConvertLogs
{
    Task<ConvertLogsResponse> RunAsync(ConvertLogsRequest request, CancellationToken cancellationToken = default);
}
