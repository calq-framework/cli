using CalqFramework.Cli;
using CalqFramework.Cli.DataAccess.InterfaceComponents;
using System;
using System.Text.Json;
using CloudProvider;

try {
    var result = new CommandLineInterface() {
        CliComponentStoreFactory = new CliComponentStoreFactory() {
            EnableShadowing = true
        }
    }.Execute(new CloudManager());

    switch (result) {
        case ResultVoid:
            break;
        case string str:
            Console.WriteLine(str);
            break;
        case object obj:
            Console.WriteLine(JsonSerializer.Serialize(obj));
            break;
    }
}
catch (CliException ex) {
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
}
