namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Constants;

public static class ApplicationConstants
{
    public const string DefaultProvider = "MINHA CDN";
    public const string DefaultVersion = "1.0";
    public const string InputFileName = "input.log";
    public const string OutputFileName = "output.log";

    public static class CacheStatus
    {
        public const string Invalidate = "INVALIDATE";
        public const string RefreshHit = "REFRESH_HIT";
        public const string Hit = "HIT";
        public const string Miss = "MISS";
    }

    public static class HttpMethods
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
    }
}
