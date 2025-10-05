namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

public record CdnLogExit(string Provider, string HttpMethod, int StatusCode, string UriPath, int TimeTaken, int ResponseSize, string CacheStatus);