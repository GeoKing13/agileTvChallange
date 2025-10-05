using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Adapter.Mapping;

public sealed class ConvertLog : IConvertLine<CdnLogEntry, CdnLogExit>
{
    private readonly CdnSettings _cdnSettings;

    public ConvertLog(CdnSettings cdnSettings)
    {
        _cdnSettings = cdnSettings ?? throw new ArgumentNullException(nameof(cdnSettings));
    }

    public CdnLogExit Convert(CdnLogEntry input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var cacheStatus = GetCacheStatus(input.CacheStatus);
        var roundedTime = (int)Math.Round(input.TimeTaken, MidpointRounding.AwayFromZero);
        return new CdnLogExit(_cdnSettings.Provider, input.HttpMethod, input.StatusCode, input.UriPath, roundedTime, input.ResponseSize, cacheStatus);
    }

    private string GetCacheStatus(string cacheStatus)
    {
        switch (cacheStatus)
        {
            case "INVALIDATE":
                return "INVALIDATE";
            default:
                return cacheStatus;
        }
    }
}
