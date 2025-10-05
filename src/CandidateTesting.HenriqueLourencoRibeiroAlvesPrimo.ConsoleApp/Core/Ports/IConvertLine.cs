namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.Core.Ports;

public interface IConvertLine<TIn, TOut>
{
    /// <summary>
    /// Convert the input line to the output line
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    TOut Convert(TIn input);
}
