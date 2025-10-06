namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

public interface ILineParser<T>
{
    bool TryParse(string line, out T? result);
}