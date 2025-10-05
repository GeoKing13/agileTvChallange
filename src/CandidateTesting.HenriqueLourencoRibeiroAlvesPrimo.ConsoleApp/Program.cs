using Microsoft.Extensions.Configuration;

// Configurar a aplicação
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Ler o provider do appsettings
var provider = configuration["CdnSettings:Provider"];

Console.WriteLine("Hello World");
Console.WriteLine($"Provider configurado: {provider}");
