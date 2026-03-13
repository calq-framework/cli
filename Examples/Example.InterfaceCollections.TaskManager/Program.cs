using CalqFramework.Cli;
using InterfaceCollectionsExample;

try {
    var result = new CommandLineInterface().Execute(new TaskManagerCli());

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
