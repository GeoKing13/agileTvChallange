namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Constants;

public static class RegexPatterns
{
    public const string CdnLogLine =
        @"^(?<responseSize>\d+)\|(?<statusCode>\d+)\|(?<cacheStatus>\w+)\|""(?<method>\w+)\s+(?<path>\S+)\s+HTTP/\d\.\d""\|(?<timeTaken>[\d.]+)$";
}
