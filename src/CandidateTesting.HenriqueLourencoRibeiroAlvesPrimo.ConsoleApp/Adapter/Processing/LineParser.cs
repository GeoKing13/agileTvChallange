using System.Globalization;
using System.Text.RegularExpressions;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Constants;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Processing;

public sealed class MinhaCdnLineParser : ILineParser<CdnLogEntry>
{
    private static readonly Regex LineRegex = new(
        RegexPatterns.CdnLogLine,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public bool TryParse(string line, out CdnLogEntry? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        var match = LineRegex.Match(line);
        if (!match.Success)
            return false;

        try
        {
            var responseSize = int.Parse(match.Groups["responseSize"].Value, CultureInfo.InvariantCulture);
            var statusCode = int.Parse(match.Groups["statusCode"].Value, CultureInfo.InvariantCulture);
            var cacheStatus = match.Groups["cacheStatus"].Value;
            var method = match.Groups["method"].Value;
            var uriPath = match.Groups["path"].Value;
            var timeTaken = double.Parse(match.Groups["timeTaken"].Value, CultureInfo.InvariantCulture);

            result = new CdnLogEntry(
                ResponseSize: responseSize,
                StatusCode: statusCode,
                CacheStatus: cacheStatus,
                HttpMethod: method,
                UriPath: uriPath,
                TimeTaken: timeTaken
            );

            return true;
        }
        catch
        {
            return false;
        }
    }
}