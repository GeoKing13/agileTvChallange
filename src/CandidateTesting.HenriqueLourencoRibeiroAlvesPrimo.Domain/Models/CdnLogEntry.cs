namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

public record CdnLogEntry(int ResponseSize, int StatusCode, string CacheStatus, string HttpMethod, string UriPath, double TimeTaken);
