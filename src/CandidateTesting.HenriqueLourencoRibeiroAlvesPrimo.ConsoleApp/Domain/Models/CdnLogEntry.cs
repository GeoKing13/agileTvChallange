namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

public sealed record CdnLogEntry(int ResponseSize, int StatusCode, string CacheStatus, string HttpMethod, string UriPath, double TimeTaken);
