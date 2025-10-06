using Xunit;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Processing;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Tests.Adapter.Processing;

public class LineParserTests
{
    private readonly MinhaCdnLineParser _parser;

    public LineParserTests()
    {
        _parser = new MinhaCdnLineParser();
    }

    [Fact]
    public void TryParse_WithValidLine_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"312|200|HIT|""GET /robots.txt HTTP/1.1""|100.2";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal(312, parsedEntry!.ResponseSize);
        Assert.Equal(200, parsedEntry.StatusCode);
        Assert.Equal("HIT", parsedEntry.CacheStatus);
        Assert.Equal("GET", parsedEntry.HttpMethod);
        Assert.Equal("/robots.txt", parsedEntry.UriPath);
        Assert.Equal(100.2, parsedEntry.TimeTaken);
    }

    [Fact]
    public void TryParse_WithMissStatus_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"101|200|MISS|""POST /myImages HTTP/1.1""|319.4";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal(101, parsedEntry!.ResponseSize);
        Assert.Equal(200, parsedEntry.StatusCode);
        Assert.Equal("MISS", parsedEntry.CacheStatus);
        Assert.Equal("POST", parsedEntry.HttpMethod);
        Assert.Equal("/myImages", parsedEntry.UriPath);
        Assert.Equal(319.4, parsedEntry.TimeTaken);
    }

    [Fact]
    public void TryParse_WithInvalidateStatus_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"312|200|INVALIDATE|""GET /robots.txt HTTP/1.1""|245.1";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal(312, parsedEntry!.ResponseSize);
        Assert.Equal(200, parsedEntry.StatusCode);
        Assert.Equal("INVALIDATE", parsedEntry.CacheStatus);
        Assert.Equal("GET", parsedEntry.HttpMethod);
        Assert.Equal("/robots.txt", parsedEntry.UriPath);
        Assert.Equal(245.1, parsedEntry.TimeTaken);
    }

    [Fact]
    public void TryParse_With404Status_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"199|404|MISS|""GET /not-found HTTP/1.1""|142.9";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal(199, parsedEntry!.ResponseSize);
        Assert.Equal(404, parsedEntry.StatusCode);
        Assert.Equal("MISS", parsedEntry.CacheStatus);
        Assert.Equal("GET", parsedEntry.HttpMethod);
        Assert.Equal("/not-found", parsedEntry.UriPath);
        Assert.Equal(142.9, parsedEntry.TimeTaken);
    }

    [Fact]
    public void TryParse_WithEmptyLine_ShouldReturnFalse()
    {
        // Arrange
        var line = "";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Fact]
    public void TryParse_WithNullLine_ShouldReturnFalse()
    {
        // Arrange
        string? line = null;

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Fact]
    public void TryParse_WithInvalidFormat_ShouldReturnFalse()
    {
        // Arrange
        var line = "invalid|format|line";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Fact]
    public void TryParse_WithMissingQuotes_ShouldReturnFalse()
    {
        // Arrange
        var line = @"312|200|HIT|GET /robots.txt HTTP/1.1|100.2";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Fact]
    public void TryParse_WithInvalidNumbers_ShouldReturnFalse()
    {
        // Arrange
        var line = @"abc|200|HIT|""GET /robots.txt HTTP/1.1""|100.2";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Fact]
    public void TryParse_WithInvalidTimeTaken_ShouldReturnFalse()
    {
        // Arrange
        var line = @"312|200|HIT|""GET /robots.txt HTTP/1.1""|abc";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.False(result);
        Assert.Null(parsedEntry);
    }

    [Theory]
    [InlineData(@"0|200|HIT|""GET /test HTTP/1.1""|0.0")]
    [InlineData(@"999999|404|MISS|""POST /api HTTP/1.1""|999.9")]
    [InlineData(@"1|500|INVALIDATE|""PUT /update HTTP/1.1""|0.1")]
    public void TryParse_WithEdgeCaseValues_ShouldReturnTrueAndCorrectData(string line)
    {
        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
    }

    [Fact]
    public void TryParse_WithComplexPath_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"123|200|HIT|""GET /api/v1/users/123/profile HTTP/1.1""|45.6";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal("/api/v1/users/123/profile", parsedEntry!.UriPath);
    }

    [Fact]
    public void TryParse_WithQueryParameters_ShouldReturnTrueAndCorrectData()
    {
        // Arrange
        var line = @"456|200|MISS|""GET /search?q=test&page=1 HTTP/1.1""|78.3";

        // Act
        var result = _parser.TryParse(line, out var parsedEntry);

        // Assert
        Assert.True(result);
        Assert.NotNull(parsedEntry);
        Assert.Equal("/search?q=test&page=1", parsedEntry!.UriPath);
    }
}
