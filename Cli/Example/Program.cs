using CalqFramework.Cli;
using System.Text.Json;

var result = CommandLineInterface.Execute(new Classlib());
if (result != null) {
    Console.WriteLine(JsonSerializer.Serialize(result));
}
