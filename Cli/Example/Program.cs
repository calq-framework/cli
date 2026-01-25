using CalqFramework.Cli;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using System;
using System.Text.Json;

var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        EnableShadowing = true
    }
}.Execute(new CloudProviderTool.CloudProvider());

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
