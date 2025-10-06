using System.ComponentModel.DataAnnotations;

namespace CandidateTesting.HenriqueLourencoRibeiroAlvesPrimo.ConsoleApp.Configuration;

public class CdnSettings
{
    [Required]
    [MinLength(1, ErrorMessage = "Provider não pode ser vazio")]
    [MaxLength(50, ErrorMessage = "Provider não pode ter mais de 50 caracteres")]
    public string Provider { get; set; } = string.Empty;
}
