namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

public interface ISourceReader
{
    IAsyncEnumerable<string> ReadLinesAsync(string sourcePath, CancellationToken cancellationToken = default);
}
