[![NuGet Version](https://img.shields.io/nuget/v/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
# Calq CLI
Calq CLI automates development of command-line tools. It interprets CLI commands, making it possible to operate on any classlib directly from the command-line without requiring any programming, through a fully customizable CLI.

## No programming required
Calq CLI in its default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and should be able to process any classlib out of the box with few limitations.
Support for overloaded methods, generic methods, and any other features not available in conventional frameworks is under consideration.

## Why Use Calq CLI: Comparison with System.CommandLine
Compared using [https://github.com/calq-framework/cli/tree/main/Cli/Example](https://github.com/calq-framework/cli/tree/main/Cli/Example)

### Calq CLI
```
﻿using CalqFramework.Cli;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using System;
using System.Text.Json;

var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        EnableShadowing = true
    }
}.Execute(new CloudProviderCLI.RootModule());

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
```

### System.CommandLine
The following code was generated with Gemini 2.5 Pro.  
The build fails with 117 errors, compiled with Visual Studio 2022 using .NET 9.
```csharp
using CloudProviderCLI;
using System.CommandLine;
using System.Text.Json;

namespace CloudProviderCLIApp;

public class Program {
    public static async Task<int> Main(string[] args) {
        var rootCommand = new RootCommand("A sample CLI for a cloud provider based on Classlib.cs");

        // Instantiate the root module from Classlib.cs
        var rootModule = new RootModule();

        // === Root Commands: add, remove ===
        var addApiKeyOption = new Option<string>("--api-key", "The API key to save.") {
            IsRequired = true
        };
        var addCommand = new Command("add", "Permanently saves the API key.")
        {
            addApiKeyOption
        };
        addCommand.SetHandler((apiKey) => {
            rootModule.Add(apiKey);
            Console.WriteLine("API Key saved successfully.");
        }, addApiKeyOption);

        var removeCommand = new Command("remove", "Removes the saved API key.");
        removeCommand.SetHandler(() => {
            rootModule.Remove();
            Console.WriteLine("API Key removed successfully.");
        });

        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(removeCommand);

        // === Add top-level submodules: compute, storage, network ===
        rootCommand.AddCommand(BuildComputeCommand());
        rootCommand.AddCommand(BuildStorageCommand());
        rootCommand.AddCommand(BuildNetworkCommand());

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>Helper method to serialize and print the result object as indented JSON.</summary>
    private static void PrintResult(object result) {
        var options = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(JsonSerializer.Serialize(result, options));
    }

    #region Compute Builders

    /// <summary>Builds the 'compute' command and all its subcommands and options.</summary>
    private static Command BuildComputeCommand() {
        var computeCommand = new Command("compute", "Manage compute resources.");

        // Add commands and options from the abstract base class
        AddAbstractComputeCommands(computeCommand, () => new ComputeModule());

        // Add commands specific to the top-level ComputeModule
        var runActionArgument = new Argument<string>("action", () => "default", "The action to perform.");
        var runCommand = new Command("run", "Runs a generic compute action.") { runActionArgument };
        runCommand.SetHandler(context => {
            var action = context.ParseResult.GetValueForArgument(runActionArgument);
            Console.WriteLine(new ComputeModule().Run(action));
        });
        computeCommand.AddCommand(runCommand);

        // Add submodules: vm, container
        var vmCommand = new Command("vm", "Manage virtual machines.");
        AddAbstractComputeCommands(vmCommand, () => new VirtualMachineModule());

        var vmNameArgument = new Argument<string>("vm-name", () => "defaultVM", "Name of the virtual machine.");
        var vmDeployCommand = new Command("deploy", "Deploys a virtual machine.") { vmNameArgument };
        vmDeployCommand.SetHandler(context => Console.WriteLine(new VirtualMachineModule().Deploy(context.ParseResult.GetValueForArgument(vmNameArgument))));
        vmCommand.AddCommand(vmDeployCommand);

        var vmIdArgument = new Argument<string>("vm-id", () => "vm-001", "ID of the virtual machine.");
        var vmStatusCommand = new Command("status", "Gets the status of a virtual machine.") { vmIdArgument };
        vmStatusCommand.SetHandler(context => Console.WriteLine(new VirtualMachineModule().Status(context.ParseResult.GetValueForArgument(vmIdArgument))));
        vmCommand.AddCommand(vmStatusCommand);

        var containerCommand = new Command("container", "Manage containers.");
        AddAbstractComputeCommands(containerCommand, () => new ContainerModule());

        var containerNameArgument = new Argument<string>("container-name", () => "defaultContainer", "Name of the container.");
        var containerDeployCommand = new Command("deploy", "Deploys a container.") { containerNameArgument };
        containerDeployCommand.SetHandler(context => Console.WriteLine(new ContainerModule().Deploy(context.ParseResult.GetValueForArgument(containerNameArgument))));
        containerCommand.AddCommand(containerDeployCommand);

        var containerIdArgument = new Argument<string>("container-id", () => "container-001", "ID of the container.");
        var containerStatusCommand = new Command("status", "Gets the status of a container.") { containerIdArgument };
        containerStatusCommand.SetHandler(context => Console.WriteLine(new ContainerModule().Status(context.ParseResult.GetValueForArgument(containerIdArgument))));
        containerCommand.AddCommand(containerStatusCommand);

        computeCommand.AddCommand(vmCommand);
        computeCommand.AddCommand(containerCommand);

        return computeCommand;
    }

    /// <summary>Adds common commands and options from AbstractComputeSubmodule to a command.</summary>
    private static void AddAbstractComputeCommands<T>(Command command, Func<T> moduleFactory) where T : AbstractComputeSubmodule {
        var maxInstancesOption = new Option<int>("--max-instances", "Maximum number of compute instances.");
        var computeRegionOption = new Option<string>("--compute-region", "Default compute region.");
        var isOptimizedOption = new Option<bool>("--is-optimized", "Indicates if compute is optimized.");
        var osOption = new Option<string>("--os", "Compute operating system type.");
        var architectureOption = new Option<string>("--architecture", "Compute architecture type.");
        command.AddOption(maxInstancesOption);
        command.AddOption(computeRegionOption);
        command.AddOption(isOptimizedOption);
        command.AddOption(osOption);
        command.AddOption(architectureOption);

        Action<T, InvocationContext> configureModule = (module, context) => {
            if (context.ParseResult.HasOption(maxInstancesOption)) module.MaxInstances = context.ParseResult.GetValueForOption(maxInstancesOption);
            if (context.ParseResult.HasOption(computeRegionOption)) module.ComputeRegion = context.ParseResult.GetValueForOption(computeRegionOption);
            if (context.ParseResult.HasOption(isOptimizedOption)) module.IsOptimized = context.ParseResult.GetValueForOption(isOptimizedOption);
            if (context.ParseResult.HasOption(osOption)) module.OperatingSystem = context.ParseResult.GetValueForOption(osOption);
            if (context.ParseResult.HasOption(architectureOption)) module.Architecture = context.ParseResult.GetValueForOption(architectureOption);
        };

        var instanceIdArgument = new Argument<string>("instance-id", () => "default", "The instance identifier.");
        var startInstanceCommand = new Command("start-instance", "Starts a compute instance.") { instanceIdArgument };
        startInstanceCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.StartInstance(context.ParseResult.GetValueForArgument(instanceIdArgument)));
        });
        command.AddCommand(startInstanceCommand);

        var stopInstanceCommand = new Command("stop-instance", "Stops a compute instance.") { instanceIdArgument };
        stopInstanceCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.StopInstance(context.ParseResult.GetValueForArgument(instanceIdArgument)));
        });
        command.AddCommand(stopInstanceCommand);

        var listInstancesCommand = new Command("list-instances", "Lists all compute instances.");
        listInstancesCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.ListInstances());
        });
        command.AddCommand(listInstancesCommand);

        var getStatusCommand = new Command("get-status", "Gets the status of compute resources.");
        getStatusCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.GetStatus());
        });
        command.AddCommand(getStatusCommand);

        var sizeArgument = new Argument<int>("size", "New size for compute resources.");
        var resizeCommand = new Command("resize-resources", "Resizes compute resources.") { sizeArgument };
        resizeCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.ResizeResources(context.ParseResult.GetValueForArgument(sizeArgument)));
        });
        command.AddCommand(resizeCommand);
    }

    #endregion

    #region Storage Builders

    /// <summary>Builds the 'storage' command and all its subcommands and options.</summary>
    private static Command BuildStorageCommand() {
        var storageCommand = new Command("storage", "Manage storage resources.");
        AddAbstractStorageCommands(storageCommand, () => new StorageModule());

        var runActionArgument = new Argument<string>("action", () => "default", "The action to perform.");
        var runCommand = new Command("run", "Runs a generic storage action.") { runActionArgument };
        runCommand.SetHandler(context => Console.WriteLine(new StorageModule().Run(context.ParseResult.GetValueForArgument(runActionArgument))));
        storageCommand.AddCommand(runCommand);

        // Submodules: blob, file
        var blobCommand = new Command("blob", "Manage blob storage.");
        AddAbstractStorageCommands(blobCommand, () => new BlobModule());
        var filePathArgument = new Argument<string>("file-path", () => "file.txt", "Path of the file to upload.");
        var blobUploadCommand = new Command("upload", "Uploads a file to the blob container.") { filePathArgument };
        blobUploadCommand.SetHandler(context => Console.WriteLine(new BlobModule().Upload(context.ParseResult.GetValueForArgument(filePathArgument))));
        blobCommand.AddCommand(blobUploadCommand);
        var blobNameArgument = new Argument<string>("blob-name", () => "defaultBlob", "Name of the blob to download.");
        var blobDownloadCommand = new Command("download", "Downloads a blob from the container.") { blobNameArgument };
        blobDownloadCommand.SetHandler(context => Console.WriteLine(new BlobModule().Download(context.ParseResult.GetValueForArgument(blobNameArgument))));
        blobCommand.AddCommand(blobDownloadCommand);

        var fileCommand = new Command("file", "Manage file storage.");
        AddAbstractStorageCommands(fileCommand, () => new FileModule());
        var fileUploadCommand = new Command("upload", "Uploads a file to the directory.") { filePathArgument };
        fileUploadCommand.SetHandler(context => Console.WriteLine(new FileModule().Upload(context.ParseResult.GetValueForArgument(filePathArgument))));
        fileCommand.AddCommand(fileUploadCommand);
        var fileNameArgument = new Argument<string>("file-name", () => "defaultFile", "Name of the file to download.");
        var fileDownloadCommand = new Command("download", "Downloads a file from the directory.") { fileNameArgument };
        fileDownloadCommand.SetHandler(context => Console.WriteLine(new FileModule().Download(context.ParseResult.GetValueForArgument(fileNameArgument))));
        fileCommand.AddCommand(fileDownloadCommand);

        storageCommand.AddCommand(blobCommand);
        storageCommand.AddCommand(fileCommand);

        return storageCommand;
    }

    /// <summary>Adds common commands and options from AbstractStorageSubmodule to a command.</summary>
    private static void AddAbstractStorageCommands<T>(Command command, Func<T> moduleFactory) where T : AbstractStorageSubmodule {
        var storageTypeOption = new Option<string>("--storage-type", "Type of storage.");
        var maxCapacityOption = new Option<int>("--max-capacity", "Maximum storage capacity in GB.");
        var storageRegionOption = new Option<string>("--storage-region", "Default storage region.");
        var activeAccountsOption = new Option<int>("--active-accounts", "Number of active storage accounts.");
        var isEncryptedOption = new Option<bool>("--is-encrypted", "Indicates if the storage is encrypted.");
        command.AddOption(storageTypeOption);
        command.AddOption(maxCapacityOption);
        command.AddOption(storageRegionOption);
        command.AddOption(activeAccountsOption);
        command.AddOption(isEncryptedOption);

        Action<T, InvocationContext> configureModule = (module, context) => {
            if (context.ParseResult.HasOption(storageTypeOption)) module.StorageType = context.ParseResult.GetValueForOption(storageTypeOption);
            if (context.ParseResult.HasOption(maxCapacityOption)) module.MaxCapacity = context.ParseResult.GetValueForOption(maxCapacityOption);
            if (context.ParseResult.HasOption(storageRegionOption)) module.StorageRegion = context.ParseResult.GetValueForOption(storageRegionOption);
            if (context.ParseResult.HasOption(activeAccountsOption)) module.ActiveAccounts = context.ParseResult.GetValueForOption(activeAccountsOption);
            if (context.ParseResult.HasOption(isEncryptedOption)) module.IsEncrypted = context.ParseResult.GetValueForOption(isEncryptedOption);
        };

        var filePathArgument = new Argument<string>("file-path", () => "default.txt", "Path to the file.");
        var uploadFileCommand = new Command("upload-file", "Uploads a file to storage.") { filePathArgument };
        uploadFileCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.UploadFile(context.ParseResult.GetValueForArgument(filePathArgument)));
        });
        command.AddCommand(uploadFileCommand);

        var fileNameArgument = new Argument<string>("file-name", () => "default.txt", "Name of the file.");
        var downloadFileCommand = new Command("download-file", "Downloads a file from storage.") { fileNameArgument };
        downloadFileCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.DownloadFile(context.ParseResult.GetValueForArgument(fileNameArgument)));
        });
        command.AddCommand(downloadFileCommand);

        var deleteFileCommand = new Command("delete-file", "Deletes a file from storage.") { fileNameArgument };
        deleteFileCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.DeleteFile(context.ParseResult.GetValueForArgument(fileNameArgument)));
        });
        command.AddCommand(deleteFileCommand);

        var listAccountsCommand = new Command("list-accounts", "Lists all storage accounts.");
        listAccountsCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.ListAccounts());
        });
        command.AddCommand(listAccountsCommand);

        var getStorageStatusCommand = new Command("get-storage-status", "Gets the storage status.");
        getStorageStatusCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.GetStorageStatus());
        });
        command.AddCommand(getStorageStatusCommand);
    }

    #endregion

    #region Network Builders

    /// <summary>Builds the 'network' command and all its subcommands and options.</summary>
    private static Command BuildNetworkCommand() {
        var networkCommand = new Command("network", "Manage network resources.");
        AddAbstractNetworkCommands(networkCommand, () => new NetworkModule());

        var runActionArgument = new Argument<string>("action", () => "default", "The action to perform.");
        var runCommand = new Command("run", "Runs a generic network action.") { runActionArgument };
        runCommand.SetHandler(context => Console.WriteLine(new NetworkModule().Run(context.ParseResult.GetValueForArgument(runActionArgument))));
        networkCommand.AddCommand(runCommand);

        // Submodules: vnet, lb, firewall
        var vnetCommand = new Command("vnet", "Manage virtual networks.");
        AddAbstractNetworkCommands(vnetCommand, () => new VirtualNetworkModule());
        var networkNameArgument = new Argument<string>("network-name", () => "defaultVNet", "Name of the virtual network.");
        var vnetCreateCommand = new Command("create", "Creates a virtual network.") { networkNameArgument };
        vnetCreateCommand.SetHandler(context => Console.WriteLine(new VirtualNetworkModule().Create(context.ParseResult.GetValueForArgument(networkNameArgument))));
        vnetCommand.AddCommand(vnetCreateCommand);
        var vnetDeleteCommand = new Command("delete", "Deletes a virtual network.") { networkNameArgument };
        vnetDeleteCommand.SetHandler(context => Console.WriteLine(new VirtualNetworkModule().Delete(context.ParseResult.GetValueForArgument(networkNameArgument))));
        vnetCommand.AddCommand(vnetDeleteCommand);

        var lbCommand = new Command("lb", "Manage load balancers.");
        AddAbstractNetworkCommands(lbCommand, () => new LoadBalancerModule());
        var lbNameArgument = new Argument<string>("lb-name", () => "defaultLB", "Name of the load balancer.");
        var lbCreateCommand = new Command("create", "Creates a load balancer.") { lbNameArgument };
        lbCreateCommand.SetHandler(context => Console.WriteLine(new LoadBalancerModule().Create(context.ParseResult.GetValueForArgument(lbNameArgument))));
        lbCommand.AddCommand(lbCreateCommand);
        var lbIdArgument = new Argument<string>("lb-id", () => "lb-001", "ID of the load balancer.");
        var lbStatusCommand = new Command("status", "Gets the status of the load balancer.") { lbIdArgument };
        lbStatusCommand.SetHandler(context => Console.WriteLine(new LoadBalancerModule().Status(context.ParseResult.GetValueForArgument(lbIdArgument))));
        lbCommand.AddCommand(lbStatusCommand);

        var firewallCommand = new Command("firewall", "Manage firewalls.");
        AddAbstractNetworkCommands(firewallCommand, () => new FirewallModule());
        var firewallIdArgument = new Argument<string>("firewall-id", () => "fw-001", "ID of the firewall.");
        var fwEnableCommand = new Command("enable", "Enables the firewall.") { firewallIdArgument };
        fwEnableCommand.SetHandler(context => Console.WriteLine(new FirewallModule().Enable(context.ParseResult.GetValueForArgument(firewallIdArgument))));
        firewallCommand.AddCommand(fwEnableCommand);
        var fwDisableCommand = new Command("disable", "Disables the firewall.") { firewallIdArgument };
        fwDisableCommand.SetHandler(context => Console.WriteLine(new FirewallModule().Disable(context.ParseResult.GetValueForArgument(firewallIdArgument))));
        firewallCommand.AddCommand(fwDisableCommand);

        networkCommand.AddCommand(vnetCommand);
        networkCommand.AddCommand(lbCommand);
        networkCommand.AddCommand(firewallCommand);

        return networkCommand;
    }

    /// <summary>Adds common commands and options from AbstractNetworkSubmodule to a command.</summary>
    private static void AddAbstractNetworkCommands<T>(Command command, Func<T> moduleFactory) where T : AbstractNetworkSubmodule {
        var networkTypeOption = new Option<string>("--network-type", "Network type.");
        var networkRegionOption = new Option<string>("--network-region", "Default network region.");
        var isConnectedOption = new Option<bool>("--is-connected", "Network connectivity status.");
        var maxBandwidthOption = new Option<int>("--max-bandwidth", "Maximum bandwidth in Mbps.");
        var endpointCountOption = new Option<int>("--endpoint-count", "Number of configured endpoints.");
        command.AddOption(networkTypeOption);
        command.AddOption(networkRegionOption);
        command.AddOption(isConnectedOption);
        command.AddOption(maxBandwidthOption);
        command.AddOption(endpointCountOption);

        Action<T, InvocationContext> configureModule = (module, context) => {
            if (context.ParseResult.HasOption(networkTypeOption)) module.NetworkType = context.ParseResult.GetValueForOption(networkTypeOption);
            if (context.ParseResult.HasOption(networkRegionOption)) module.NetworkRegion = context.ParseResult.GetValueForOption(networkRegionOption);
            if (context.ParseResult.HasOption(isConnectedOption)) module.IsConnected = context.ParseResult.GetValueForOption(isConnectedOption);
            if (context.ParseResult.HasOption(maxBandwidthOption)) module.MaxBandwidth = context.ParseResult.GetValueForOption(maxBandwidthOption);
            if (context.ParseResult.HasOption(endpointCountOption)) module.EndpointCount = context.ParseResult.GetValueForOption(endpointCountOption);
        };

        var endpointNameArgument = new Argument<string>("endpoint-name", () => "defaultEndpoint", "Name of the endpoint.");
        var createEndpointCommand = new Command("create-endpoint", "Creates a network endpoint.") { endpointNameArgument };
        createEndpointCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.CreateEndpoint(context.ParseResult.GetValueForArgument(endpointNameArgument)));
        });
        command.AddCommand(createEndpointCommand);

        var deleteEndpointCommand = new Command("delete-endpoint", "Deletes a network endpoint.") { endpointNameArgument };
        deleteEndpointCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.DeleteEndpoint(context.ParseResult.GetValueForArgument(endpointNameArgument)));
        });
        command.AddCommand(deleteEndpointCommand);

        var listEndpointsCommand = new Command("list-endpoints", "Lists all network endpoints.");
        listEndpointsCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.ListEndpoints());
        });
        command.AddCommand(listEndpointsCommand);

        var getNetworkStatusCommand = new Command("get-network-status", "Gets the network status.");
        getNetworkStatusCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.GetNetworkStatus());
        });
        command.AddCommand(getNetworkStatusCommand);

        var testSpeedCommand = new Command("test-speed", "Tests network speed.");
        testSpeedCommand.SetHandler(context => {
            var module = moduleFactory();
            configureModule(module, context);
            PrintResult(module.TestSpeed());
        });
        command.AddCommand(testSpeedCommand);
    }

    #endregion
}
```

## Customization Features
Every logical component is separated into a module that is part of the CLI configuration.
- **API Stringification**  
The default stringifier uses kebab-case conversion and CliNameAttribute for multi-aliasing.
- **Case-sensitive/insensitive API Access Validation**  
Access conditions are defined by BindingFlags and validators with MemberInfo context.
- **Value Conversion/Validation**  
Conversion is global for all values, supporting strings, primitive types, and lists.
- **Option Parsing**  
The reader respects GNU conventions and extends them with negative options using '+' and '++'.
- **API Accessors**  
Default accessors for fields, properties, and methods integrate all related modules.
- **Help Menu**  
Help printer receives CLI components constructed by accessors with all metadata.  
Descriptions for the help menu can be provided using XML documentation or custom IHelpPrinter.  
The availability of CliDescriptionAttribute is yet to be decided.
##### Custom Help Menu Quick Start
```csharp
// CLI components also include additional data, such as values or MemberInfo
public class HelpPrinter : IHelpPrinter {
    public void PrintHelp(Type rootSubmoduleType, Submodule submodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        PrintHelp(submodules, subcommands, options);
    }

    public void PrintHelp(Type rootSubmoduleType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        PrintHelp(submodules, subcommands, options);
    }

    private void PrintHelp(IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        var sections = new SectionInfo[] {
            new("Submodules", submodules.Select(x => new ItemInfo(x.Keys))),
            new("Subcommands", subcommands.Select(x => new ItemInfo(x.Keys))),
            new("Options", options.Select(x => new ItemInfo(x.Keys.Select(GetOptionName))))
        };
        PrintSections(sections);
    }

    public void PrintSubcommandHelp(Type rootSubmoduleType, Subcommand subcommand, IEnumerable<Option> options) {
        var sections = new SectionInfo[] {
            new("Parameters", subcommand.Parameters.Select(x => new ItemInfo(x.Keys.Select(GetOptionName)))),
            new("Options", options.Select(x => new ItemInfo(x.Keys.Select(GetOptionName))))
        };
        PrintSections(sections);
    }

    private void PrintSections(IEnumerable<SectionInfo> sections) {
        foreach (var section in sections) {
            Console.WriteLine(section.Title);
            foreach (var item in section.ItemInfos) {
                Console.WriteLine(string.Join(" ", item.Keys));
            }
        }
    }

    private static string GetOptionName(string key) => key.Length > 1 ? $"--{key}" : $"-{key}";

    private record ItemInfo(IEnumerable<string> Keys);
    private record SectionInfo(string Title, IEnumerable<ItemInfo> ItemInfos);
}
```

## Usage
No specific coding convention is necessary and CliNameAttribute is optional.
  
The following will interpret the command-line arguments, execute any underlying API, and return the result.
```csharp
var result = new CommandLineInterface.Execute(new Classlib());
```
To enable descriptions based on XML documentation, add the following to the project file.
```
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
Method parameters can be enabled to shadow fields and properties as follows:
```csharp
var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        EnableShadowing = true
    }
}.Execute(new Classlib());
```
To just populate any object with options use OptionDeserializer:
```csharp
OptionDeserializer.Deserialize(settingsObject)
```
### Demo Example
[https://github.com/calq-framework/cli/tree/main/Cli/Example](https://github.com/calq-framework/cli/tree/main/Cli/Example)

![SubmoduleHelpExample](https://github.com/calq-framework/cli/blob/main/Cli/Example/SubmoduleHelpExample.png?raw=true)  
![SubcommandHelpExample](https://github.com/calq-framework/cli/blob/main/Cli/Example/SubcommandHelpExample.png?raw=true)

### Quick Start
```csharp
using CalqFramework.Cli;
using CalqFramework.Cli.Serialization;
using System;
using System.Text.Json;

var result = new CommandLineInterface().Execute(new QuickStart());
if (result is not ResultVoid) Console.WriteLine(JsonSerializer.Serialize(result));

/// <summary>Displayed in the help menu.</summary>
class QuickStart {
    /// <summary>Displayed in the help menu.</summary>
    public SubStart Submodule { get; } = new SubStart();
    public void QuickRun() {}
}
class SubStart {
    public string DefaultValue { get; set; } = "default";
    [CliName("run")] // rename from 'sub-run' to 'run'
    [CliName("r")] // add alias 'r'
    public QuickResult SubRun(int requiredParameter, int optionalParameter = 1)
        => new QuickResult(DefaultValue, requiredParameter, optionalParameter);
}
public record QuickResult(string s, int a, int b);
```

## License
Calq CLI is dual-licensed under the GNU AGPLv3 and a commercial license.
