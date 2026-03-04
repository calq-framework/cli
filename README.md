[![NuGet Version](https://img.shields.io/nuget/v/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CalqFramework.Cli?color=508cf0)](https://www.nuget.org/packages/CalqFramework.Cli)
[![REUSE status](https://api.reuse.software/badge/github.com/calq-framework/cli)](https://api.reuse.software/info/github.com/calq-framework/cli)

# Calq CLI
Calq CLI automates development of command-line tools. It interprets CLI commands, making it possible to operate on any classlib directly from the command-line without requiring any programming, through a fully customizable CLI.

## No programming required
Calq CLI in its default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and processes any classlib out of the box with comprehensive type support.

Supports list-like collection types from `System.Collections` and `System.Collections.Generic` namespaces with automatic concrete type mapping for collection interfaces. Dictionary types are not supported.

## A Radically Simpler Approach

### Calq CLI vs. System.CommandLine
| Feature | Calq CLI | System.CommandLine |
| :--- | :--- | :--- |
| **CLI Definition** | Auto-generated from Classlib | Fluent API |
| **Option Sources** | Fields + Properties + Method Parameters | Method Parameters |
| **Help Documentation** | XML Docs + Code | Code |
| **Completion Protocols** | Cobra + dotnet-suggest | dotnet-suggest |
| **Custom Completion** | Delegate + Class-based | Delegate + Class-based |
| **Enum Completion** | ✅ | ❌ |
| **Infer Subcommands from Methods** | ✅ | ❌ |
| **Infer Options from Properties/Fields/Parameters** | ✅ | ❌ |
| **Infer Multi-Value Options from Collections** | ✅ | ❌ |
| **Deserialize CLI Args to Objects** | ✅ | ❌ |
| **Learning Curve** | Low | Moderate |
| **Development Time** | Very Fast | Moderate |

### Code Comparison
Both examples implement CLI for the classlib from:  
[https://github.com/calq-framework/cli/tree/main/Examples/Example.NestedSubmodules.CloudProvider](https://github.com/calq-framework/cli/tree/main/Examples/Example.NestedSubmodules.CloudProvider)

### Calq CLI
The following template is a complete implementation.
```csharp
using CalqFramework.Cli;
using CalqFramework.Cli.DataAccess;
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
```

### System.CommandLine
The following code was generated with AI using Gemini 2.5 Pro.  
The build failed with 170 errors, compiled with Visual Studio 2022 using .NET 9.
```csharp
using System.CommandLine;
using CloudProviderTool;

// Helper Methods
static T CreateModule<T>(string? apiKey) where T : SubmoduleBase, new()
{
    var module = new T();
    if (!string.IsNullOrEmpty(apiKey))
// ... another 132 lines of code

var rootCommand = new RootCommand("A CLI for managing cloud provider resources.");

// Global Options and Root Commands
var apiKeyOption = new Option<string>("--api-key", "API Key used for authentication. Overrides saved key.");
rootCommand.AddGlobalOption(apiKeyOption);

var addCommand = new Command("add", "Permanently saves an API key for the CLI.")
// ... another 19 lines of code

// Compute Module
var computeCommand = new Command("compute", "Manage compute resources.");
AddComputeOptions(computeCommand);

var computeRunCommand = new Command("run", "Runs a generic compute action.")
// ... another 50 lines of code

// Storage Module
var storageCommand = new Command("storage", "Manage storage resources.");
AddStorageOptions(storageCommand);

var storageRunCommand = new Command("run", "Runs a generic storage action.") { new Argument<string>("action", () => "default", "The action to perform.") };
// ... another 61 lines of code

// Network Module
var networkCommand = new Command("network", "Manage network resources.");
AddNetworkOptions(networkCommand);

var networkRunCommand = new Command("run", "Runs a generic network action.") { new Argument<string>("action", () => "default", "The action to perform.") };
// ... another 77 lines of code

return await rootCommand.InvokeAsync(args);
```

## Usage
No specific coding convention is necessary and CliNameAttribute is optional.
  
The following will interpret the command-line arguments, execute any underlying API, and return the result.
```csharp
var result = new CommandLineInterface().Execute(new Classlib());
```

### Built-in Help and Version
Help is automatically available with `--help` or `-h` at any level (root, submodule, or subcommand).
Version is automatically available with `--version` or `-v`.

To enable descriptions based on XML documentation, add the following to the project file:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

### Configuration Options

**Enable Shadowing** - Method parameters can shadow fields and properties:
```csharp
var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        EnableShadowing = true
    }
}.Execute(new Classlib());
```

**Skip Unknown Options** - Continue execution instead of throwing exceptions for unknown options:
```csharp
var result = new CommandLineInterface() {
    SkipUnknown = true
}.Execute(new Classlib());
```

**Custom Arguments** - Execute with custom arguments instead of environment args:
```csharp
var result = new CommandLineInterface().Execute(new Classlib(), new[] { "subcommand", "--option", "value" });
```

**Access Control** - Control which members are exposed:
```csharp
var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        AccessFields = false,      // Only properties, no fields
        AccessProperties = true
    }
}.Execute(new Classlib());
```

**Custom Binding Flags** - Control member visibility and case sensitivity:
```csharp
var result = new CommandLineInterface() {
    CliComponentStoreFactory = new CliComponentStoreFactory() {
        BindingFlags = BindingFlags.Instance | BindingFlags.Public,  // Case-sensitive
        MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
    }
}.Execute(new Classlib());
```

### OptionDeserializer
To populate any object with options without full CLI execution:
```csharp
OptionDeserializer.Deserialize(settingsObject);

// With custom arguments
OptionDeserializer.Deserialize(settingsObject, new[] { "--option", "value" });

// Skip unknown options
OptionDeserializer.Deserialize(settingsObject, new OptionDeserializerConfiguration { 
    SkipUnknown = true 
});
```
## Shell Completion
Calq CLI provides automatic shell completion for commands, options, and parameters across multiple shells.

### Automatic Completion
Completion works automatically for:
- **Submodules and subcommands** - All available commands at any level
- **Options and parameters** - All flags and their values
- **Enums** - All enum values with case-insensitive matching
- **Booleans** - `true` and `false` values
- **Collections** - Collection types from `System.Collections` and `System.Collections.Generic` (lists, sets, arrays, etc.) with element type completion (enums, bools, etc.)
- **File paths** - Files via `FileInfo`
- **Directory paths** - Directories via `DirectoryInfo`
- **File system paths** - Both files and directories via `FileSystemInfo`

### Custom Completion
Use `[CliCompletion]` attribute for custom completion providers:

**Method-based completion** - Call an instance method:

```csharp
[CliCompletion("GetRegions")]
public string Region { get; set; } = "us-east-1";

private IEnumerable<string> GetRegions(string partialInput) {
    var regions = new[] { "us-east-1", "us-west-2", "eu-west-1" };
    return regions.Where(r => r.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
}
```

**Custom provider** - Implement `ICompletionProvider`:
```csharp
public class RegionCompletionProvider : ICompletionProvider {
    public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
        var regions = new[] { "us-east-1", "us-west-2", "eu-west-1" };
        return regions.Where(r => r.StartsWith(context.PartialInput, StringComparison.OrdinalIgnoreCase));
    }
}

[CliCompletion(typeof(RegionCompletionProvider))]
public string Region { get; set; }
```

**Built-in providers** - Use framework-provided completion providers:
```csharp
// File completion with extension filtering
[CliCompletion(typeof(FileCompletionProvider), "*.json;*.yaml")]
public string ConfigFile { get; set; }

// Directory completion
[CliCompletion(typeof(DirectoryCompletionProvider))]
public string OutputDir { get; set; }

// File system completion (files and directories)
[CliCompletion(typeof(FileSystemCompletionProvider))]
public string Path { get; set; }
```

### Shell Installation

```bash
# Generate script
mycli completion bash
mycli completion zsh
mycli completion powershell
mycli completion pwsh
mycli completion fish

# Install (creates completion file and updates shell profile)
mycli completion bash install
mycli completion zsh install
mycli completion powershell install
mycli completion pwsh install
mycli completion fish install

# Uninstall
mycli completion bash uninstall

# All shells at once
mycli completion all
mycli completion all install
mycli completion all uninstall
```

After installation, restart your shell or source the profile.

### Completion Protocols

**Cobra-style** - `__complete` command (used by generated shell scripts):
```bash
mycli __complete
```

**dotnet-suggest** - Microsoft's completion protocol for .NET tools:
```bash
dotnet tool install -g dotnet-suggest
```

### Demo Examples
[Interface Collections Example](https://github.com/calq-framework/cli/tree/main/Examples/Example.InterfaceCollections.TaskManager)  

[Autocomplete Example](https://github.com/calq-framework/cli/tree/main/Examples/Example.Autocomplete.CloudProvider)  

[Nested Submodules Example](https://github.com/calq-framework/cli/tree/main/Examples/Example.NestedSubmodules.CloudProvider)  

![SubmoduleHelpExample](https://github.com/calq-framework/cli/blob/main/Examples/Example.NestedSubmodules.CloudProvider/SubmoduleHelpExample.png?raw=true)  
![SubcommandHelpExample](https://github.com/calq-framework/cli/blob/main/Examples/Example.NestedSubmodules.CloudProvider/SubcommandHelpExample.png?raw=true)

### Quick Start
```bash
dotnet add package CalqFramework.Cli
```

```csharp
using CalqFramework.Cli;
using System;
using System.Text.Json;

try {
    var result = new CommandLineInterface().Execute(new QuickStart());
    if (result is not ResultVoid) Console.WriteLine(JsonSerializer.Serialize(result));
}
catch (CliException ex) {
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
}

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
Calq CLI is dual-licensed under GNU AGPLv3 and the Calq Commercial License.

