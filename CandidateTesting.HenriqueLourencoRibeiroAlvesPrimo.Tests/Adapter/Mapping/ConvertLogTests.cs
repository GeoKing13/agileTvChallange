using Xunit;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Adapter.Mapping;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Tests.Adapter.Mapping;

public class ConvertLogTests
{
    private readonly ConvertLog _convertLog;
    private readonly CdnSettings _cdnSettings;

    public ConvertLogTests()
    {
        _cdnSettings = new CdnSettings { Provider = "MINHA CDN" };
        _convertLog = new ConvertLog(_cdnSettings);
    }

    [Fact]
    public void Convert_WithValidInput_ShouldReturnCorrectOutput()
    {
        // Arrange
        var input = new CdnLogEntry(312, 200, "HIT", "GET", "/robots.txt", 100.2);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal("MINHA CDN", result.Provider);
        Assert.Equal("GET", result.HttpMethod);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("/robots.txt", result.UriPath);
        Assert.Equal(100, result.TimeTaken); // Rounded
        Assert.Equal(312, result.ResponseSize);
        Assert.Equal("HIT", result.CacheStatus);
    }

    [Fact]
    public void Convert_WithInvalidateCacheStatus_ShouldReturnRefreshHit()
    {
        // Arrange
        var input = new CdnLogEntry(312, 200, "INVALIDATE", "GET", "/robots.txt", 245.1);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal("REFRESH_HIT", result.CacheStatus);
    }

    [Fact]
    public void Convert_WithMissCacheStatus_ShouldReturnMiss()
    {
        // Arrange
        var input = new CdnLogEntry(101, 200, "MISS", "POST", "/myImages", 319.4);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal("MISS", result.CacheStatus);
    }

    [Fact]
    public void Convert_WithHitCacheStatus_ShouldReturnHit()
    {
        // Arrange
        var input = new CdnLogEntry(312, 200, "HIT", "GET", "/robots.txt", 100.2);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal("HIT", result.CacheStatus);
    }

    [Fact]
    public void Convert_WithTimeTakenRoundedUp_ShouldRoundCorrectly()
    {
        // Arrange
        var input = new CdnLogEntry(100, 200, "HIT", "GET", "/test", 100.6);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal(101, result.TimeTaken);
    }

    [Fact]
    public void Convert_WithTimeTakenRoundedDown_ShouldRoundCorrectly()
    {
        // Arrange
        var input = new CdnLogEntry(100, 200, "HIT", "GET", "/test", 100.4);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal(100, result.TimeTaken);
    }

    [Fact]
    public void Convert_WithNegativeInput_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _convertLog.Convert(null!));
    }

    [Theory]
    [InlineData(0, 200, "HIT", "GET", "/test", 100.0)]
    [InlineData(999999, 404, "MISS", "POST", "/api/data", 0.1)]
    [InlineData(1, 500, "INVALIDATE", "PUT", "/update", 999.9)]
    public void Convert_WithEdgeCaseValues_ShouldHandleCorrectly(int responseSize, int statusCode, string cacheStatus, string httpMethod, string uriPath, double timeTaken)
    {
        // Arrange
        var input = new CdnLogEntry(responseSize, statusCode, cacheStatus, httpMethod, uriPath, timeTaken);

        // Act
        var result = _convertLog.Convert(input);

        // Assert
        Assert.Equal(responseSize, result.ResponseSize);
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(httpMethod, result.HttpMethod);
        Assert.Equal(uriPath, result.UriPath);
        Assert.Equal((int)Math.Round(timeTaken, MidpointRounding.AwayFromZero), result.TimeTaken);

        if (cacheStatus == "INVALIDATE")
            Assert.Equal("REFRESH_HIT", result.CacheStatus);
        else
            Assert.Equal(cacheStatus, result.CacheStatus);
    }
}
