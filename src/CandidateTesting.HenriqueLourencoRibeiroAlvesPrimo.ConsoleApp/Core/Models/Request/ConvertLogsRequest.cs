namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Request;

/// <summary>
/// DTO for useCase entry
/// </summary>
/// <param name="SourceUrl"></param>
/// <param name="TargetPath"></param>
public record ConvertLogsRequest(string SourceUrl, string TargetPath);
