using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.In.Sources;

public sealed class FileSourceReader : ISourceReader
{
    public async IAsyncEnumerable<string> ReadLinesAsync(string sourcePath, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"File not found: {sourcePath}");
        }

        using var reader = new StreamReader(sourcePath);
        while (!reader.EndOfStream)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is not null)
            {
                yield return line;
            }
        }
    }
}