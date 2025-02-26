![NuGet Version](https://img.shields.io/nuget/v/CalqFramework.Cli?color=508cf0&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FCalqFramework.Cli)
![NuGet Downloads](https://img.shields.io/nuget/dt/CalqFramework.Cli?color=508cf0&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FCalqFramework.Cli)
# Calq CLI
Calq CLI automates development of command-line tools. It interprets CLI commands, making it possible to operate on any classlib directly from the command-line without requiring any programming, through a fully customizable CLI.

## No programming required
Calq CLI in its default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and should be able to process any classlib out of the box with few limitations.
Support for overloaded methods, generic methods, and any other missing features is under consideration.

## Customization Features
Every logical component is separated into a module that is part of the CLI configuration.
- **API Stringification**  
The default stringifier uses kebab-case conversion and CliNameAttribute for multi-aliasing.
- **Case-sensitive/insensitive API Access Validation**  
Access conditions are defined by BindingFlags and validators with MemberInfo context.
- **Value Conversion/Validation**  
Conversion is global for all values supporting strings, primitive types, and lists.
- **Option Parsing**  
The reader respects GNU conventions and extends them with negative options using '+' and '++'.
- **API Accessors**  
Default accessors for fields, properties, and methods integrate all related modules.
- **Help Menu**  
Help printer receives CLI components constructed by accessors with all metadata.  
Descriptions for the help menu can be provided using XML documentation or custom IHelpPrinter.  
The availability of CliDescriptionAttribute is yet to be decided.
##### Custom Help Menu Quick Start
<details>
  <summary>Click to show.</summary>
  
```csharp
// CLI components also include additional data, such as values or MemberInfo, which are not shown in this example.
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
</details>

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
### Individual License
The availability will be confirmed at a later time.
### Corporate License
The availability will be confirmed at a later time.
