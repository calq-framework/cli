using System.Text.Json;
using CalqFramework.Cli;
using Example.InterfaceCollections.TaskController;

try {
    object result = new CommandLineInterface().Execute(new TaskControllerManager());

    switch (result) {
        case ValueTuple:
            break;
        case string str:
            Console.WriteLine(str);
            break;
        case object obj:
            Console.WriteLine(JsonSerializer.Serialize(obj));
            break;
    }
} catch (CliException ex) {
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
}
