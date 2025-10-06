using System.IO;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Validation;

public static class InputValidator
{
    public static void ValidateFileExists(string filePath, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException($"O caminho do arquivo não pode ser vazio.", parameterName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
    }

    public static void ValidateDirectoryWritable(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            return;

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Teste de escrita
            var testFile = Path.Combine(directoryPath, ".write_test");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"Diretório não é gravável: {directoryPath}", ex);
        }
    }

    public static void ValidateLogLine(string line, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new ArgumentException($"Linha {lineNumber} está vazia.");

        if (line.Length > 1000)
            throw new ArgumentException($"Linha {lineNumber} é muito longa (máximo 1000 caracteres).");
    }
}
