using AutocompleteExample;
using CalqFramework.Cli;

try {
    var result = new CommandLineInterface().Execute(new CloudProviderCli());

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
