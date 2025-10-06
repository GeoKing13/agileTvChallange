using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;


public interface ITargetWriter
{
    Task WriteHeaderAsync(DateTimeOffset date, string targetPath, CancellationToken ct = default);

    Task AppendAsync(CdnLogExit entry, string targetPath, CancellationToken ct = default);
}
