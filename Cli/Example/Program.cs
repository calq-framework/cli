using CalqFramework.Cli;
using System.Text.Json;

var result = new CommandLineInterface().Execute(new Classlib());
if (result != null) {
    Console.WriteLine(JsonSerializer.Serialize(result));
}
