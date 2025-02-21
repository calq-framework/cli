using CalqFramework.Cli;
using System.Text.Json;

var result = new CommandLineInterface().Execute(new CloudProviderCLI.RootModule());
if (result != null) {
    if (result is string) {
        Console.WriteLine(result);
    } else {
        Console.WriteLine(JsonSerializer.Serialize(result));
    }
}
