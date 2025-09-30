[![NuGet Version](https://img.shields.io/nuget/v/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
# Calq CLI
Calq CLI automates development of command-line tools. It interprets CLI commands, making it possible to operate on any classlib directly from the command-line without requiring any programming, through a fully customizable CLI.

## No programming required
Calq CLI in its default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and should be able to process any classlib out of the box with few limitations.
Support for overloaded methods, generic methods, and any other features not available in conventional frameworks is under consideration.

## Why Calq CLI: Comparison with System.CommandLine
Both examples implement CLI for the classlib from:  
[https://github.com/calq-framework/cli/tree/main/Cli/Example](https://github.com/calq-framework/cli/tree/main/Cli/Example)

### Calq CLI
The following template is a complete implementation.
```csharp
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
The following code was generated with AI using Gemini 2.5 Pro.  
The build fails with 170 errors, compiled with Visual Studio 2022 using .NET 9.
```csharp
using System.CommandLine;
using CloudProviderCLI;

#region Helper Methods

// Creates a new instance of a submodule and sets its API key.
static T CreateModule<T>(string? apiKey) where T : SubmoduleBase, new()
{
    var module = new T();
    if (!string.IsNullOrEmpty(apiKey))
    {
        module.ApiKey = apiKey;
    }
    return module;
}

// Helper to add common options for all submodules inheriting from AbstractComputeSubmodule
static void AddComputeOptions(Command command)
{
    command.AddOption(new Option<int>("--max-instances", () => 10, "Maximum number of compute instances."));
    command.AddOption(new Option<string>("--compute-region", () => "us-west", "Default compute region."));
    command.AddOption(new Option<bool>("--optimized", () => true, "Indicates if compute is optimized."));
    command.AddOption(new Option<string>("--os", () => "Linux", "Compute operating system type."));
    command.AddOption(new Option<string>("--architecture", () => "x86_64", "Compute architecture type."));
}

// Helper to bind common compute options to a module instance
static void BindComputeOptions(AbstractComputeSubmodule module, int maxInstances, string computeRegion, bool isOptimized, string os, string architecture)
{
    module.MaxInstances = maxInstances;
    module.ComputeRegion = computeRegion;
    module.IsOptimized = isOptimized;
    module.OperatingSystem = os;
    module.Architecture = architecture;
}

// Helper to add common commands for all submodules inheriting from AbstractComputeSubmodule
static void AddComputeCommands<T>(Command command, Option<string> apiKeyOption, Option<int> maxInstancesOption, Option<string> computeRegionOption, Option<bool> isOptimizedOption, Option<string> osOption, Option<string> architectureOption) where T : AbstractComputeSubmodule, new()
{
    var startInstanceCommand = new Command("start-instance", "Starts a compute instance.")
    {
        new Option<string>("--instance-id", () => "default", "Instance identifier.")
    };
    startInstanceCommand.SetHandler((apiKey, instanceId, maxInstances, computeRegion, isOptimized, os, architecture) =>
    {
        var module = CreateModule<T>(apiKey);
        BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
        Console.WriteLine(module.StartInstance(instanceId));
    }, apiKeyOption, startInstanceCommand.Options.OfType<Option<string>>().First(), maxInstancesOption, computeRegionOption, isOptimizedOption, osOption, architectureOption);

    var stopInstanceCommand = new Command("stop-instance", "Stops a compute instance.")
    {
        new Option<string>("--instance-id", () => "default", "Instance identifier.")
    };
    stopInstanceCommand.SetHandler((apiKey, instanceId, maxInstances, computeRegion, isOptimized, os, architecture) =>
    {
        var module = CreateModule<T>(apiKey);
        BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
        Console.WriteLine(module.StopInstance(instanceId));
    }, apiKeyOption, stopInstanceCommand.Options.OfType<Option<string>>().First(), maxInstancesOption, computeRegionOption, isOptimizedOption, osOption, architectureOption);

    var listInstancesCommand = new Command("list-instances", "Lists all compute instances.");
    listInstancesCommand.SetHandler((apiKey, maxInstances, computeRegion, isOptimized, os, architecture) =>
    {
        var module = CreateModule<T>(apiKey);
        BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
        Console.WriteLine(module.ListInstances());
    }, apiKeyOption, maxInstancesOption, computeRegionOption, isOptimizedOption, osOption, architectureOption);

    var getStatusCommand = new Command("get-status", "Gets the status of compute resources.");
    getStatusCommand.SetHandler((apiKey, maxInstances, computeRegion, isOptimized, os, architecture) =>
    {
        var module = CreateModule<T>(apiKey);
        BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
        Console.WriteLine(module.GetStatus());
    }, apiKeyOption, maxInstancesOption, computeRegionOption, isOptimizedOption, osOption, architectureOption);

    var resizeResourcesCommand = new Command("resize-resources", "Resizes compute resources.")
    {
        new Argument<int>("size", "New size for compute resources.")
    };
    resizeResourcesCommand.SetHandler((apiKey, size, maxInstances, computeRegion, isOptimized, os, architecture) =>
    {
        var module = CreateModule<T>(apiKey);
        BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
        Console.WriteLine(module.ResizeResources(size));
    }, apiKeyOption, resizeResourcesCommand.Arguments.OfType<Argument<int>>().First(), maxInstancesOption, computeRegionOption, isOptimizedOption, osOption, architectureOption);

    command.AddCommand(startInstanceCommand);
    command.AddCommand(stopInstanceCommand);
    command.AddCommand(listInstancesCommand);
    command.AddCommand(getStatusCommand);
    command.AddCommand(resizeResourcesCommand);
}


// Helper to add common options for all submodules inheriting from AbstractStorageSubmodule
static void AddStorageOptions(Command command)
{
    command.AddOption(new Option<string>("--storage-type", () => "Blob", "Type of storage."));
    command.AddOption(new Option<int>("--max-capacity", () => 500, "Maximum storage capacity in GB."));
    command.AddOption(new Option<string>("--storage-region", () => "us-east", "Default storage region."));
    command.AddOption(new Option<int>("--active-accounts", () => 5, "Number of active storage accounts."));
    command.AddOption(new Option<bool>("--encrypted", () => true, "Indicates if the storage is encrypted."));
}

// Helper to bind common storage options to a module instance
static void BindStorageOptions(AbstractStorageSubmodule module, string storageType, int maxCapacity, string storageRegion, int activeAccounts, bool isEncrypted)
{
    module.StorageType = storageType;
    module.MaxCapacity = maxCapacity;
    module.StorageRegion = storageRegion;
    module.ActiveAccounts = activeAccounts;
    module.IsEncrypted = isEncrypted;
}

// Helper to add common commands for all submodules inheriting from AbstractStorageSubmodule
static void AddStorageCommands<T>(Command command, Option<string> apiKeyOption, Option<string> storageTypeOption, Option<int> maxCapacityOption, Option<string> storageRegionOption, Option<int> activeAccountsOption, Option<bool> isEncryptedOption) where T : AbstractStorageSubmodule, new()
{
    var uploadFileCommand = new Command("upload-file", "Uploads a file to storage.") { new Option<string>("--file-path", () => "default.txt", "Path to the file.") };
    uploadFileCommand.SetHandler((apiKey, filePath, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
    {
        var module = CreateModule<T>(apiKey);
        BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
        Console.WriteLine(module.UploadFile(filePath));
    }, apiKeyOption, uploadFileCommand.Options.First(), storageTypeOption, maxCapacityOption, storageRegionOption, activeAccountsOption, isEncryptedOption);

    var downloadFileCommand = new Command("download-file", "Downloads a file from storage.") { new Option<string>("--file-name", () => "default.txt", "Name of the file.") };
    downloadFileCommand.SetHandler((apiKey, fileName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
    {
        var module = CreateModule<T>(apiKey);
        BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
        Console.WriteLine(module.DownloadFile(fileName));
    }, apiKeyOption, downloadFileCommand.Options.First(), storageTypeOption, maxCapacityOption, storageRegionOption, activeAccountsOption, isEncryptedOption);

    var deleteFileCommand = new Command("delete-file", "Deletes a file from storage.") { new Option<string>("--file-name", () => "default.txt", "Name of the file.") };
    deleteFileCommand.SetHandler((apiKey, fileName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
    {
        var module = CreateModule<T>(apiKey);
        BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
        Console.WriteLine(module.DeleteFile(fileName));
    }, apiKeyOption, deleteFileCommand.Options.First(), storageTypeOption, maxCapacityOption, storageRegionOption, activeAccountsOption, isEncryptedOption);

    var listAccountsCommand = new Command("list-accounts", "Lists all storage accounts.");
    listAccountsCommand.SetHandler((apiKey, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
    {
        var module = CreateModule<T>(apiKey);
        BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
        Console.WriteLine(module.ListAccounts());
    }, apiKeyOption, storageTypeOption, maxCapacityOption, storageRegionOption, activeAccountsOption, isEncryptedOption);

    var getStorageStatusCommand = new Command("get-storage-status", "Gets the storage status.");
    getStorageStatusCommand.SetHandler((apiKey, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
    {
        var module = CreateModule<T>(apiKey);
        BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
        Console.WriteLine(module.GetStorageStatus());
    }, apiKeyOption, storageTypeOption, maxCapacityOption, storageRegionOption, activeAccountsOption, isEncryptedOption);

    command.AddCommand(uploadFileCommand);
    command.AddCommand(downloadFileCommand);
    command.AddCommand(deleteFileCommand);
    command.AddCommand(listAccountsCommand);
    command.AddCommand(getStorageStatusCommand);
}


// Helper to add common options for all submodules inheriting from AbstractNetworkSubmodule
static void AddNetworkOptions(Command command)
{
    command.AddOption(new Option<string>("--network-type", () => "Virtual", "Network type."));
    command.AddOption(new Option<string>("--network-region", () => "us-central", "Default network region."));
    command.AddOption(new Option<bool>("--connected", () => true, "Indicates network connectivity status."));
    command.AddOption(new Option<int>("--max-bandwidth", () => 1000, "Maximum bandwidth in Mbps."));
    command.AddOption(new Option<int>("--endpoint-count", () => 10, "Number of configured endpoints."));
}

// Helper to bind common network options to a module instance
static void BindNetworkOptions(AbstractNetworkSubmodule module, string networkType, string networkRegion, bool isConnected, int maxBandwidth, int endpointCount)
{
    module.NetworkType = networkType;
    module.NetworkRegion = networkRegion;
    module.IsConnected = isConnected;
    module.MaxBandwidth = maxBandwidth;
    module.EndpointCount = endpointCount;
}

// Helper to add common commands for all submodules inheriting from AbstractNetworkSubmodule
static void AddNetworkCommands<T>(Command command, Option<string> apiKeyOption, Option<string> networkTypeOption, Option<string> networkRegionOption, Option<bool> isConnectedOption, Option<int> maxBandwidthOption, Option<int> endpointCountOption) where T : AbstractNetworkSubmodule, new()
{
    var createEndpointCommand = new Command("create-endpoint", "Creates a network endpoint.") { new Option<string>("--endpoint-name", () => "defaultEndpoint", "Name of the endpoint.") };
    createEndpointCommand.SetHandler((apiKey, endpointName, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
    {
        var module = CreateModule<T>(apiKey);
        BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
        Console.WriteLine(module.CreateEndpoint(endpointName));
    }, apiKeyOption, createEndpointCommand.Options.First(), networkTypeOption, networkRegionOption, isConnectedOption, maxBandwidthOption, endpointCountOption);

    var deleteEndpointCommand = new Command("delete-endpoint", "Deletes a network endpoint.") { new Option<string>("--endpoint-name", () => "defaultEndpoint", "Name of the endpoint.") };
    deleteEndpointCommand.SetHandler((apiKey, endpointName, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
    {
        var module = CreateModule<T>(apiKey);
        BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
        Console.WriteLine(module.DeleteEndpoint(endpointName));
    }, apiKeyOption, deleteEndpointCommand.Options.First(), networkTypeOption, networkRegionOption, isConnectedOption, maxBandwidthOption, endpointCountOption);

    var listEndpointsCommand = new Command("list-endpoints", "Lists all network endpoints.");
    listEndpointsCommand.SetHandler((apiKey, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
    {
        var module = CreateModule<T>(apiKey);
        BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
        Console.WriteLine(module.ListEndpoints());
    }, apiKeyOption, networkTypeOption, networkRegionOption, isConnectedOption, maxBandwidthOption, endpointCountOption);

    var getNetworkStatusCommand = new Command("get-network-status", "Gets the network status.");
    getNetworkStatusCommand.SetHandler((apiKey, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
    {
        var module = CreateModule<T>(apiKey);
        BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
        Console.WriteLine(module.GetNetworkStatus());
    }, apiKeyOption, networkTypeOption, networkRegionOption, isConnectedOption, maxBandwidthOption, endpointCountOption);

    var testSpeedCommand = new Command("test-speed", "Tests network speed.");
    testSpeedCommand.SetHandler((apiKey, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
    {
        var module = CreateModule<T>(apiKey);
        BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
        Console.WriteLine(module.TestSpeed());
    }, apiKeyOption, networkTypeOption, networkRegionOption, isConnectedOption, maxBandwidthOption, endpointCountOption);

    command.AddCommand(createEndpointCommand);
    command.AddCommand(deleteEndpointCommand);
    command.AddCommand(listEndpointsCommand);
    command.AddCommand(getNetworkStatusCommand);
    command.AddCommand(testSpeedCommand);
}


#endregion

var rootCommand = new RootCommand("A CLI for managing cloud provider resources.");

#region Global Options and Root Commands

var apiKeyOption = new Option<string>("--api-key", "API Key used for authentication. Overrides saved key.");
rootCommand.AddGlobalOption(apiKeyOption);

var addCommand = new Command("add", "Permanently saves an API key for the CLI.")
{
    new Argument<string>("api-key-value", "The API key to save.")
};
addCommand.SetHandler((apiKeyValue) =>
{
    new RootModule().Add(apiKeyValue);
    Console.WriteLine("API key saved successfully.");
}, addCommand.Arguments.OfType<Argument<string>>().First());

var removeCommand = new Command("remove", "Removes the saved API key.");
removeCommand.SetHandler(() =>
{
    new RootModule().Remove();
    Console.WriteLine("API key removed successfully.");
});

rootCommand.AddCommand(addCommand);
rootCommand.AddCommand(removeCommand);

#endregion

#region Compute Module

var computeCommand = new Command("compute", "Manage compute resources.");
AddComputeOptions(computeCommand);

var computeRunCommand = new Command("run", "Runs a generic compute action.")
{
    new Argument<string>("action", () => "default", "The action to perform.")
};
computeRunCommand.SetHandler((apiKey, action, maxInstances, computeRegion, isOptimized, os, architecture) =>
{
    var module = CreateModule<ComputeModule>(apiKey);
    BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
    Console.WriteLine(module.Run(action));
}, apiKeyOption, computeRunCommand.Arguments.First(), computeCommand.Options[0] as Option<int>, computeCommand.Options[1] as Option<string>, computeCommand.Options[2] as Option<bool>, computeCommand.Options[3] as Option<string>, computeCommand.Options[4] as Option<string>);
computeCommand.AddCommand(computeRunCommand);
AddComputeCommands<ComputeModule>(computeCommand, apiKeyOption, computeCommand.Options[0] as Option<int>, computeCommand.Options[1] as Option<string>, computeCommand.Options[2] as Option<bool>, computeCommand.Options[3] as Option<string>, computeCommand.Options[4] as Option<string>);

// Compute -> Virtual Machine Submodule
var vmCommand = new Command("vm", "Manage virtual machine resources.");
AddComputeOptions(vmCommand);
AddComputeCommands<VirtualMachineModule>(vmCommand, apiKeyOption, vmCommand.Options[0] as Option<int>, vmCommand.Options[1] as Option<string>, vmCommand.Options[2] as Option<bool>, vmCommand.Options[3] as Option<string>, vmCommand.Options[4] as Option<string>);

var vmDeployCommand = new Command("deploy", "Deploys a virtual machine.") { new Option<string>("--vm-name", () => "defaultVM", "Name of the VM.") };
vmDeployCommand.SetHandler((apiKey, vmName, maxInstances, computeRegion, isOptimized, os, architecture) =>
{
    var module = CreateModule<VirtualMachineModule>(apiKey);
    BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
    Console.WriteLine(module.Deploy(vmName));
}, apiKeyOption, vmDeployCommand.Options.First(), vmCommand.Options[0] as Option<int>, vmCommand.Options[1] as Option<string>, vmCommand.Options[2] as Option<bool>, vmCommand.Options[3] as Option<string>, vmCommand.Options[4] as Option<string>);
vmCommand.AddCommand(vmDeployCommand);

var vmStatusCommand = new Command("status", "Gets the status of a virtual machine.") { new Option<string>("--vm-id", () => "vm-001", "ID of the VM.") };
vmStatusCommand.SetHandler((apiKey, vmId, maxInstances, computeRegion, isOptimized, os, architecture) =>
{
    var module = CreateModule<VirtualMachineModule>(apiKey);
    BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
    Console.WriteLine(module.Status(vmId));
}, apiKeyOption, vmStatusCommand.Options.First(), vmCommand.Options[0] as Option<int>, vmCommand.Options[1] as Option<string>, vmCommand.Options[2] as Option<bool>, vmCommand.Options[3] as Option<string>, vmCommand.Options[4] as Option<string>);
vmCommand.AddCommand(vmStatusCommand);
computeCommand.AddCommand(vmCommand);

// Compute -> Container Submodule
var containerCommand = new Command("container", "Manage container resources.");
AddComputeOptions(containerCommand);
AddComputeCommands<ContainerModule>(containerCommand, apiKeyOption, containerCommand.Options[0] as Option<int>, containerCommand.Options[1] as Option<string>, containerCommand.Options[2] as Option<bool>, containerCommand.Options[3] as Option<string>, containerCommand.Options[4] as Option<string>);

var containerDeployCommand = new Command("deploy", "Deploys a container.") { new Option<string>("--container-name", () => "defaultContainer", "Name of the container.") };
containerDeployCommand.SetHandler((apiKey, containerName, maxInstances, computeRegion, isOptimized, os, architecture) =>
{
    var module = CreateModule<ContainerModule>(apiKey);
    BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
    Console.WriteLine(module.Deploy(containerName));
}, apiKeyOption, containerDeployCommand.Options.First(), containerCommand.Options[0] as Option<int>, containerCommand.Options[1] as Option<string>, containerCommand.Options[2] as Option<bool>, containerCommand.Options[3] as Option<string>, containerCommand.Options[4] as Option<string>);
containerCommand.AddCommand(containerDeployCommand);

var containerStatusCommand = new Command("status", "Gets the status of a container.") { new Option<string>("--container-id", () => "container-001", "ID of the container.") };
containerStatusCommand.SetHandler((apiKey, containerId, maxInstances, computeRegion, isOptimized, os, architecture) =>
{
    var module = CreateModule<ContainerModule>(apiKey);
    BindComputeOptions(module, maxInstances, computeRegion, isOptimized, os, architecture);
    Console.WriteLine(module.Status(containerId));
}, apiKeyOption, containerStatusCommand.Options.First(), containerCommand.Options[0] as Option<int>, containerCommand.Options[1] as Option<string>, containerCommand.Options[2] as Option<bool>, containerCommand.Options[3] as Option<string>, containerCommand.Options[4] as Option<string>);
containerCommand.AddCommand(containerStatusCommand);
computeCommand.AddCommand(containerCommand);

rootCommand.AddCommand(computeCommand);

#endregion

#region Storage Module

var storageCommand = new Command("storage", "Manage storage resources.");
AddStorageOptions(storageCommand);

var storageRunCommand = new Command("run", "Runs a generic storage action.") { new Argument<string>("action", () => "default", "The action to perform.") };
storageRunCommand.SetHandler((apiKey, action, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
{
    var module = CreateModule<StorageModule>(apiKey);
    BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
    Console.WriteLine(module.Run(action));
}, apiKeyOption, storageRunCommand.Arguments.First(), storageCommand.Options[0] as Option<string>, storageCommand.Options[1] as Option<int>, storageCommand.Options[2] as Option<string>, storageCommand.Options[3] as Option<int>, storageCommand.Options[4] as Option<bool>);
storageCommand.AddCommand(storageRunCommand);
AddStorageCommands<StorageModule>(storageCommand, apiKeyOption, storageCommand.Options[0] as Option<string>, storageCommand.Options[1] as Option<int>, storageCommand.Options[2] as Option<string>, storageCommand.Options[3] as Option<int>, storageCommand.Options[4] as Option<bool>);

// Storage -> Blob Submodule
var blobCommand = new Command("blob", "Manage blob storage.");
AddStorageOptions(blobCommand);
blobCommand.AddOption(new Option<string>("--container-name", () => "defaultBlobContainer", "Name of the blob container."));
AddStorageCommands<BlobModule>(blobCommand, apiKeyOption, blobCommand.Options[0] as Option<string>, blobCommand.Options[1] as Option<int>, blobCommand.Options[2] as Option<string>, blobCommand.Options[3] as Option<int>, blobCommand.Options[4] as Option<bool>);

var blobUploadCommand = new Command("upload", "Uploads a file to a blob container.") { new Option<string>("--file-path", () => "file.txt", "Path of the file to upload.") };
blobUploadCommand.SetHandler((apiKey, filePath, containerName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
{
    var module = CreateModule<BlobModule>(apiKey);
    BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
    module.ContainerName = containerName;
    Console.WriteLine(module.Upload(filePath));
}, apiKeyOption, blobUploadCommand.Options.First(), blobCommand.Options[5] as Option<string>, blobCommand.Options[0] as Option<string>, blobCommand.Options[1] as Option<int>, blobCommand.Options[2] as Option<string>, blobCommand.Options[3] as Option<int>, blobCommand.Options[4] as Option<bool>);
blobCommand.AddCommand(blobUploadCommand);

var blobDownloadCommand = new Command("download", "Downloads a blob.") { new Option<string>("--blob-name", () => "defaultBlob", "Name of the blob to download.") };
blobDownloadCommand.SetHandler((apiKey, blobName, containerName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
{
    var module = CreateModule<BlobModule>(apiKey);
    BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
    module.ContainerName = containerName;
    Console.WriteLine(module.Download(blobName));
}, apiKeyOption, blobDownloadCommand.Options.First(), blobCommand.Options[5] as Option<string>, blobCommand.Options[0] as Option<string>, blobCommand.Options[1] as Option<int>, blobCommand.Options[2] as Option<string>, blobCommand.Options[3] as Option<int>, blobCommand.Options[4] as Option<bool>);
blobCommand.AddCommand(blobDownloadCommand);
storageCommand.AddCommand(blobCommand);

// Storage -> File Submodule
var fileCommand = new Command("file", "Manage file storage.");
AddStorageOptions(fileCommand);
fileCommand.AddOption(new Option<string>("--directory-name", () => "defaultFileDirectory", "Name of the file directory."));
AddStorageCommands<FileModule>(fileCommand, apiKeyOption, fileCommand.Options[0] as Option<string>, fileCommand.Options[1] as Option<int>, fileCommand.Options[2] as Option<string>, fileCommand.Options[3] as Option<int>, fileCommand.Options[4] as Option<bool>);

var fileUploadCommand = new Command("upload", "Uploads a file to a directory.") { new Option<string>("--file-path", () => "file.txt", "Path of the file to upload.") };
fileUploadCommand.SetHandler((apiKey, filePath, directoryName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
{
    var module = CreateModule<FileModule>(apiKey);
    BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
    module.DirectoryName = directoryName;
    Console.WriteLine(module.Upload(filePath));
}, apiKeyOption, fileUploadCommand.Options.First(), fileCommand.Options[5] as Option<string>, fileCommand.Options[0] as Option<string>, fileCommand.Options[1] as Option<int>, fileCommand.Options[2] as Option<string>, fileCommand.Options[3] as Option<int>, fileCommand.Options[4] as Option<bool>);
fileCommand.AddCommand(fileUploadCommand);

var fileDownloadCommand = new Command("download", "Downloads a file from a directory.") { new Option<string>("--file-name", () => "defaultFile", "Name of the file to download.") };
fileDownloadCommand.SetHandler((apiKey, fileName, directoryName, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted) =>
{
    var module = CreateModule<FileModule>(apiKey);
    BindStorageOptions(module, storageType, maxCapacity, storageRegion, activeAccounts, isEncrypted);
    module.DirectoryName = directoryName;
    Console.WriteLine(module.Download(fileName));
}, apiKeyOption, fileDownloadCommand.Options.First(), fileCommand.Options[5] as Option<string>, fileCommand.Options[0] as Option<string>, fileCommand.Options[1] as Option<int>, fileCommand.Options[2] as Option<string>, fileCommand.Options[3] as Option<int>, fileCommand.Options[4] as Option<bool>);
fileCommand.AddCommand(fileDownloadCommand);
storageCommand.AddCommand(fileCommand);

rootCommand.AddCommand(storageCommand);

#endregion

#region Network Module

var networkCommand = new Command("network", "Manage network resources.");
AddNetworkOptions(networkCommand);

var networkRunCommand = new Command("run", "Runs a generic network action.") { new Argument<string>("action", () => "default", "The action to perform.") };
networkRunCommand.SetHandler((apiKey, action, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<NetworkModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Run(action));
}, apiKeyOption, networkRunCommand.Arguments.First(), networkCommand.Options[0] as Option<string>, networkCommand.Options[1] as Option<string>, networkCommand.Options[2] as Option<bool>, networkCommand.Options[3] as Option<int>, networkCommand.Options[4] as Option<int>);
networkCommand.AddCommand(networkRunCommand);
AddNetworkCommands<NetworkModule>(networkCommand, apiKeyOption, networkCommand.Options[0] as Option<string>, networkCommand.Options[1] as Option<string>, networkCommand.Options[2] as Option<bool>, networkCommand.Options[3] as Option<int>, networkCommand.Options[4] as Option<int>);

// Network -> Virtual Network Submodule
var vnetCommand = new Command("vnet", "Manage virtual networks.");
AddNetworkOptions(vnetCommand);
AddNetworkCommands<VirtualNetworkModule>(vnetCommand, apiKeyOption, vnetCommand.Options[0] as Option<string>, vnetCommand.Options[1] as Option<string>, vnetCommand.Options[2] as Option<bool>, vnetCommand.Options[3] as Option<int>, vnetCommand.Options[4] as Option<int>);
var vnetCreateCommand = new Command("create", "Creates a virtual network.") { new Option<string>("--network-name", () => "defaultVNet", "Name of the virtual network.") };
vnetCreateCommand.SetHandler((apiKey, networkName, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<VirtualNetworkModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Create(networkName));
}, apiKeyOption, vnetCreateCommand.Options.First(), vnetCommand.Options[0] as Option<string>, vnetCommand.Options[1] as Option<string>, vnetCommand.Options[2] as Option<bool>, vnetCommand.Options[3] as Option<int>, vnetCommand.Options[4] as Option<int>);
vnetCommand.AddCommand(vnetCreateCommand);
var vnetDeleteCommand = new Command("delete", "Deletes a virtual network.") { new Option<string>("--network-name", () => "defaultVNet", "Name of the virtual network.") };
vnetDeleteCommand.SetHandler((apiKey, networkName, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<VirtualNetworkModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Delete(networkName));
}, apiKeyOption, vnetDeleteCommand.Options.First(), vnetCommand.Options[0] as Option<string>, vnetCommand.Options[1] as Option<string>, vnetCommand.Options[2] as Option<bool>, vnetCommand.Options[3] as Option<int>, vnetCommand.Options[4] as Option<int>);
vnetCommand.AddCommand(vnetDeleteCommand);
networkCommand.AddCommand(vnetCommand);

// Network -> Load Balancer Submodule
var lbCommand = new Command("lb", "Manage load balancers.");
AddNetworkOptions(lbCommand);
AddNetworkCommands<LoadBalancerModule>(lbCommand, apiKeyOption, lbCommand.Options[0] as Option<string>, lbCommand.Options[1] as Option<string>, lbCommand.Options[2] as Option<bool>, lbCommand.Options[3] as Option<int>, lbCommand.Options[4] as Option<int>);
var lbCreateCommand = new Command("create", "Creates a load balancer.") { new Option<string>("--lb-name", () => "defaultLB", "Name of the load balancer.") };
lbCreateCommand.SetHandler((apiKey, lbName, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<LoadBalancerModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Create(lbName));
}, apiKeyOption, lbCreateCommand.Options.First(), lbCommand.Options[0] as Option<string>, lbCommand.Options[1] as Option<string>, lbCommand.Options[2] as Option<bool>, lbCommand.Options[3] as Option<int>, lbCommand.Options[4] as Option<int>);
lbCommand.AddCommand(lbCreateCommand);
var lbStatusCommand = new Command("status", "Gets the status of a load balancer.") { new Option<string>("--lb-id", () => "lb-001", "ID of the load balancer.") };
lbStatusCommand.SetHandler((apiKey, lbId, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<LoadBalancerModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Status(lbId));
}, apiKeyOption, lbStatusCommand.Options.First(), lbCommand.Options[0] as Option<string>, lbCommand.Options[1] as Option<string>, lbCommand.Options[2] as Option<bool>, lbCommand.Options[3] as Option<int>, lbCommand.Options[4] as Option<int>);
lbCommand.AddCommand(lbStatusCommand);
networkCommand.AddCommand(lbCommand);

// Network -> Firewall Submodule
var fwCommand = new Command("firewall", "Manage firewalls.");
AddNetworkOptions(fwCommand);
AddNetworkCommands<FirewallModule>(fwCommand, apiKeyOption, fwCommand.Options[0] as Option<string>, fwCommand.Options[1] as Option<string>, fwCommand.Options[2] as Option<bool>, fwCommand.Options[3] as Option<int>, fwCommand.Options[4] as Option<int>);
var fwEnableCommand = new Command("enable", "Enables a firewall.") { new Option<string>("--firewall-id", () => "fw-001", "ID of the firewall.") };
fwEnableCommand.SetHandler((apiKey, firewallId, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<FirewallModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Enable(firewallId));
}, apiKeyOption, fwEnableCommand.Options.First(), fwCommand.Options[0] as Option<string>, fwCommand.Options[1] as Option<string>, fwCommand.Options[2] as Option<bool>, fwCommand.Options[3] as Option<int>, fwCommand.Options[4] as Option<int>);
fwCommand.AddCommand(fwEnableCommand);
var fwDisableCommand = new Command("disable", "Disables a firewall.") { new Option<string>("--firewall-id", () => "fw-001", "ID of the firewall.") };
fwDisableCommand.SetHandler((apiKey, firewallId, networkType, networkRegion, isConnected, maxBandwidth, endpointCount) =>
{
    var module = CreateModule<FirewallModule>(apiKey);
    BindNetworkOptions(module, networkType, networkRegion, isConnected, maxBandwidth, endpointCount);
    Console.WriteLine(module.Disable(firewallId));
}, apiKeyOption, fwDisableCommand.Options.First(), fwCommand.Options[0] as Option<string>, fwCommand.Options[1] as Option<string>, fwCommand.Options[2] as Option<bool>, fwCommand.Options[3] as Option<int>, fwCommand.Options[4] as Option<int>);
fwCommand.AddCommand(fwDisableCommand);
networkCommand.AddCommand(fwCommand);

rootCommand.AddCommand(networkCommand);

#endregion

return await rootCommand.InvokeAsync(args);
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



