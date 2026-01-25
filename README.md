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
The build failed with 170 errors, compiled with Visual Studio 2022 using .NET 9.
```csharp
using System.CommandLine;
using CloudProviderCLI;

// Helper Methods
static T CreateModule<T>(string? apiKey) where T : SubmoduleBase, new()
{
    var module = new T();
    if (!string.IsNullOrEmpty(apiKey))
// ... 132 lines of code

var rootCommand = new RootCommand("A CLI for managing cloud provider resources.");

// Global Options and Root Commands
var apiKeyOption = new Option<string>("--api-key", "API Key used for authentication. Overrides saved key.");
rootCommand.AddGlobalOption(apiKeyOption);

var addCommand = new Command("add", "Permanently saves an API key for the CLI.")
// ... 19 lines of code

// Compute Module
var computeCommand = new Command("compute", "Manage compute resources.");
AddComputeOptions(computeCommand);

var computeRunCommand = new Command("run", "Runs a generic compute action.")
// ... 50 lines of code

// Storage Module
var storageCommand = new Command("storage", "Manage storage resources.");
AddStorageOptions(storageCommand);

var storageRunCommand = new Command("run", "Runs a generic storage action.") { new Argument<string>("action", () => "default", "The action to perform.") };
// ... 61 lines of code

// Network Module
var networkCommand = new Command("network", "Manage network resources.");
AddNetworkOptions(networkCommand);

var networkRunCommand = new Command("run", "Runs a generic network action.") { new Argument<string>("action", () => "default", "The action to perform.") };
// ... 77 lines of code

return await rootCommand.InvokeAsync(args);
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



