using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

Console.WriteLine("Running DbUp");






var basePath = "/imagebrowser/";
var parameters = ParametersService.RetrieveParametersWithDecryption(basePath).Result;



bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

if (!isDevelopment)
{
    var configuration = new ConfigurationBuilder()
            .Build();

    try
    {
    var connectionString = parameters.Single(p => p.Name.Equals(basePath + "ProductionConnection")).Value;
    var upgrader = DeployChanges.To
                       .PostgresqlDatabase(connectionString)
                       .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                       .LogToConsole()
                       .Build();


        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }

    }
    catch
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: Connection string not found");
        Console.ResetColor();
        return -1;
    }
}

Console.ForegroundColor = ConsoleColor.Green;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;

