# Calq CLI
Calq CLI helps convert .NET libraries into command-line tools. It interprets CLI commands, making it possible to operate on any library directly from the command-line with no programming required, through a fully customizable CLI.  

## Key Feature
### No programming required.
Calq CLI in its default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and should be able to process any classlib out of the box with few limitations.
Support for overloaded methods, generic methods, and any other missing features is under consideration.

## Customization Features
The following modules are easily customizable through configuration and/or interface implementation.
### API Stringification
The default stringifier uses kebab-case conversion and CliNameAttribute for multi-aliasing.
### Case-sensitive/insensitive API Access Validation
Validation uses BindingFlags for simple configuration and MemberInfo context for complete access control.
### Value Conversion/Validation
Strings, primitive types, and IList types are supported by a single converter that validates all values.
### Option Parsing
In addition to short and long options, negative options are supported using '+' and '++'.
### API Accessors
Default accessors for fields, properties, and methods rely on all the modules mentioned above.
### Help Menu
Help printer interface receives all the context from accessors, making custom implementations failproof.  
Descriptions for the help menu can be provided using XML documentation or custom IHelpPrinter.  
Availability of CliDescriptionAttribute is yet to be decided.
#### Custom Help Menu Quick Start
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
CliNameAttribute isn't necessary.  
No specific coding convention is necessary.  
  
The following will interpret the command-line arguments, execute any underlying API, and return the result.
```csharp
var result = new CommandLineInterface.Execute(new Classlib());
```
It's also possible to just populate any object with options.
```csharp
OptionDeserializer.Deserialize(settingsObject)
```
To enable descriptions based on XML documentation, add the following to the project file.
```
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
### Demo Example
[https://github.com/calq-framework/cli/tree/main/Cli/Example](https://github.com/calq-framework/cli/tree/main/Cli/Example)

![SubmoduleHelpExample](https://github.com/calq-framework/cli/blob/main/Cli/Example/SubmoduleHelpExample.png?raw=true)  
![SubcommandHelpExample](https://github.com/calq-framework/cli/blob/main/Cli/Example/SubcommandHelpExample.png?raw=true)

### Quick Start
```csharp
using CalqFramework.Cli;
using CalqFramework.Cli.Serialization;
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
    public string DefaultValue { get; set; } = "optional";
    [CliName("sub-run")]
    [CliName("r")]
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
