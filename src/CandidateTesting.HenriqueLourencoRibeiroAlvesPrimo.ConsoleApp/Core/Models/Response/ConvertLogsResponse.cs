namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Models.Response;

/// <summary>
/// DTO for useCase exit
/// </summary>
/// <param name="TargetPath"></param>
/// <param name="LinesRead"></param>
/// <param name="LinesConverted"></param>
/// <param name="LinesInvalid"></param>
/// <param name="StartTime"></param>
/// <param name="FinishTime"></param>
/// <param name="Success"></param>
public record ConvertLogsResponse(string TargetPath, int LinesRead, int LinesConverted, int LinesInvalid, DateTime StartTime, DateTime FinishTime, bool Success);
