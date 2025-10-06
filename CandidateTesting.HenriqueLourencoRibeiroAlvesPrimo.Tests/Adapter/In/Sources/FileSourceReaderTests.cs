using Xunit;
using CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Adapter.In.Sources;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Tests.Adapter.In.Sources;

public class FileSourceReaderTests : IDisposable
{
    private readonly FileSourceReader _reader;
    private readonly string _testFilePath;
    private readonly string _emptyFilePath;
    private readonly string _nonExistentFilePath;

    public FileSourceReaderTests()
    {
        _reader = new FileSourceReader();
        _testFilePath = Path.GetTempFileName();
        _emptyFilePath = Path.GetTempFileName();
        _nonExistentFilePath = Path.Combine(Path.GetTempPath(), "non_existent_file.log");

        await File.WriteAllLinesAsync(_testFilePath, new[]
        {
            "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2",
            "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4",
            "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9",
            "312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1"
        });
    }

    [Fact]
    public async Task ReadLinesAsync_WithValidFile_ShouldReturnAllLines()
    {
        // Act
        var lines = new List<string>();
        await foreach (var line in _reader.ReadLinesAsync(_testFilePath))
        {
            lines.Add(line);
        }

        // Assert
        Assert.Equal(4, lines.Count);
        Assert.Equal("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2", lines[0]);
        Assert.Equal("101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4", lines[1]);
        Assert.Equal("199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9", lines[2]);
        Assert.Equal("312|200|INVALIDATE|\"GET /robots.txt HTTP/1.1\"|245.1", lines[3]);
    }

    [Fact]
    public async Task ReadLinesAsync_WithEmptyFile_ShouldReturnEmptyCollection()
    {
        // Arrange
        await File.WriteAllTextAsync(_emptyFilePath, string.Empty);

        // Act
        var lines = new List<string>();
        await foreach (var line in _reader.ReadLinesAsync(_emptyFilePath))
        {
            lines.Add(line);
        }

        // Assert
        Assert.Empty(lines);
    }

    [Fact]
    public async Task ReadLinesAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _reader.ReadLinesAsync(_nonExistentFilePath).GetAsyncEnumerator().MoveNextAsync().AsTask());
    }

    [Fact]
    public async Task ReadLinesAsync_WithCancellation_ShouldStopReading()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var lines = new List<string>();

        // Act
        await foreach (var line in _reader.ReadLinesAsync(_testFilePath, cts.Token))
        {
            lines.Add(line);
            if (lines.Count == 2)
            {
                cts.Cancel();
                break;
            }
        }

        // Assert
        Assert.Equal(2, lines.Count);
    }

    [Fact]
    public async Task ReadLinesAsync_WithFileContainingEmptyLines_ShouldReturnAllLinesIncludingEmpty()
    {
        // Arrange
        var fileWithEmptyLines = Path.GetTempFileName();
        await File.WriteAllLinesAsync(fileWithEmptyLines, new[]
        {
            "312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2",
            "",
            "101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4",
            "",
            "199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9"
        });

        try
        {
            // Act
            var lines = new List<string>();
            await foreach (var line in _reader.ReadLinesAsync(fileWithEmptyLines))
            {
                lines.Add(line);
            }

            // Assert
            Assert.Equal(5, lines.Count);
            Assert.Equal("312|200|HIT|\"GET /robots.txt HTTP/1.1\"|100.2", lines[0]);
            Assert.Equal("", lines[1]);
            Assert.Equal("101|200|MISS|\"POST /myImages HTTP/1.1\"|319.4", lines[2]);
            Assert.Equal("", lines[3]);
            Assert.Equal("199|404|MISS|\"GET /not-found HTTP/1.1\"|142.9", lines[4]);
        }
        finally
        {
            File.Delete(fileWithEmptyLines);
        }
    }

    [Fact]
    public async Task ReadLinesAsync_WithLargeFile_ShouldReadAllLines()
    {
        // Arrange
        var largeFilePath = Path.GetTempFileName();
        var lines = new List<string>();
        for (int i = 0; i < 1000; i++)
        {
            lines.Add($"123|200|HIT|\"GET /test{i} HTTP/1.1\"|100.0");
        }
        await File.WriteAllLinesAsync(largeFilePath, lines);

        try
        {
            // Act
            var readLines = new List<string>();
            await foreach (var line in _reader.ReadLinesAsync(largeFilePath))
            {
                readLines.Add(line);
            }

            // Assert
            Assert.Equal(1000, readLines.Count);
            Assert.Equal("123|200|HIT|\"GET /test0 HTTP/1.1\"|100.0", readLines[0]);
            Assert.Equal("123|200|HIT|\"GET /test999 HTTP/1.1\"|100.0", readLines[999]);
        }
        finally
        {
            File.Delete(largeFilePath);
        }
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
        if (File.Exists(_emptyFilePath))
            File.Delete(_emptyFilePath);
    }
}
