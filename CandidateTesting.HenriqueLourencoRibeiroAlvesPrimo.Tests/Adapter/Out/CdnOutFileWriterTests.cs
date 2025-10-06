using Xunit;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.Out;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Tests.Adapter.Out;

public class CdnOutFileWriterTests : IDisposable
{
    private readonly CdnOutFileWriter _writer;
    private readonly string _testFilePath;

    public CdnOutFileWriterTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Version"] = "1.0"
            })
            .Build();

        _writer = new CdnOutFileWriter(configuration);
        _testFilePath = Path.GetTempFileName();
    }

    [Fact]
    public async Task WriteHeaderAsync_ShouldCreateFileWithCorrectHeader()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);

        // Assert
        Assert.True(File.Exists(_testFilePath));
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(3, lines.Length);
        Assert.Equal("#Version: 1.0", lines[0]);
        Assert.Equal("#Date: 06/10/2025 14:30:00", lines[1]);
        Assert.Equal("#Fields: provider http-method status-code uri-path time-taken response-size cache-status", lines[2]);
    }

    [Fact]
    public async Task AppendAsync_ShouldAppendEntryToFile()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entry = new CdnLogExit("MINHA CDN", "GET", 200, "/robots.txt", 100, 312, "HIT");

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        await _writer.AppendAsync(entry, _testFilePath);

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(4, lines.Length);
        Assert.Equal("\"MINHA CDN\" GET 200 /robots.txt 100 312 HIT", lines[3]);
    }

    [Fact]
    public async Task AppendAsync_WithMultipleEntries_ShouldAppendAllEntries()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entries = new[]
        {
            new CdnLogExit("MINHA CDN", "GET", 200, "/robots.txt", 100, 312, "HIT"),
            new CdnLogExit("MINHA CDN", "POST", 200, "/myImages", 319, 101, "MISS"),
            new CdnLogExit("MINHA CDN", "GET", 404, "/not-found", 143, 199, "MISS")
        };

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        foreach (var entry in entries)
        {
            await _writer.AppendAsync(entry, _testFilePath);
        }

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(6, lines.Length); // 3 header lines + 3 data lines
        Assert.Equal("\"MINHA CDN\" GET 200 /robots.txt 100 312 HIT", lines[3]);
        Assert.Equal("\"MINHA CDN\" POST 200 /myImages 319 101 MISS", lines[4]);
        Assert.Equal("\"MINHA CDN\" GET 404 /not-found 143 199 MISS", lines[5]);
    }

    [Fact]
    public async Task AppendAsync_WithSpecialCharactersInPath_ShouldFormatCorrectly()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entry = new CdnLogExit("MINHA CDN", "GET", 200, "/api/v1/users/123/profile", 100, 312, "HIT");

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        await _writer.AppendAsync(entry, _testFilePath);

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("\"MINHA CDN\" GET 200 /api/v1/users/123/profile 100 312 HIT", lines[3]);
    }

    [Fact]
    public async Task AppendAsync_WithQueryParameters_ShouldFormatCorrectly()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entry = new CdnLogExit("MINHA CDN", "GET", 200, "/search?q=test&page=1", 100, 312, "HIT");

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        await _writer.AppendAsync(entry, _testFilePath);

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("\"MINHA CDN\" GET 200 /search?q=test&page=1 100 312 HIT", lines[3]);
    }

    [Fact]
    public async Task AppendAsync_WithRefreshHitStatus_ShouldFormatCorrectly()
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entry = new CdnLogExit("MINHA CDN", "GET", 200, "/robots.txt", 245, 312, "REFRESH_HIT");

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        await _writer.AppendAsync(entry, _testFilePath);

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("\"MINHA CDN\" GET 200 /robots.txt 245 312 REFRESH_HIT", lines[3]);
    }

    [Fact]
    public async Task WriteHeaderAsync_WithDirectoryPath_ShouldCreateDirectory()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "test_output");
        var filePath = Path.Combine(testDir, "output.log");
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);

        try
        {
            // Act
            await _writer.WriteHeaderAsync(date, filePath);

            // Assert
            Assert.True(Directory.Exists(testDir));
            Assert.True(File.Exists(filePath));
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Theory]
    [InlineData(0, 200, "HIT", "GET", "/test", 0, 0)]
    [InlineData(999999, 404, "MISS", "POST", "/api", 999, 999999)]
    [InlineData(1, 500, "REFRESH_HIT", "PUT", "/update", 1, 1)]
    public async Task AppendAsync_WithEdgeCaseValues_ShouldFormatCorrectly(int timeTaken, int statusCode, string cacheStatus, string httpMethod, string uriPath, int responseSize, int expectedResponseSize)
    {
        // Arrange
        var date = new DateTimeOffset(2025, 10, 6, 14, 30, 0, TimeSpan.Zero);
        var entry = new CdnLogExit("MINHA CDN", httpMethod, statusCode, uriPath, timeTaken, responseSize, cacheStatus);

        // Act
        await _writer.WriteHeaderAsync(date, _testFilePath);
        await _writer.AppendAsync(entry, _testFilePath);

        // Assert
        var content = await File.ReadAllTextAsync(_testFilePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var expectedLine = $"\"MINHA CDN\" {httpMethod} {statusCode} {uriPath} {timeTaken} {expectedResponseSize} {cacheStatus}";
        Assert.Equal(expectedLine, lines[3]);
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
}
