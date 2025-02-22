# CalqFramework.Cli
CalqFramework.Cli helps convert .NET libraries into command-line tools. It interprets CLI commands, making it possible to operate on any library directly from the command line with no programming required, through a fully customizable CLI.  

## Key Feature
### No programming required.
Cli should be able to interpret any classlib without any configuration.  
The default configuration follows GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html) and should work out of the box with few limitations.  
Support for overloaded methods and any other missing features is under consideration.

## Customization Features
The following modules are easily customizable through configuration and/or interface implementation.
### API Stringification
The default stringifier uses kebab-case conversion, with support for multi-aliasing through CliNameAttribute.
### Case-sensitive/insensitive API Access Validation
Access can be configured through BindingFlags settings and MemberInfo validators.
### Value Conversion/Validation
Strings, primitive types, and IList types are supported by a single converter that validates all values.
### Option Parsing
In addition to short and long options, negative options are supported using '+' and '++'.
### API Access
The default accessors for fields, properties, and methods rely on all the modules mentioned above.
### Help Menu
Help printer interface receives all the context from accessors, making custom implementations failproof.  
Descriptions for the help menu can be provided using XML documentation or custom IHelpPrinter.  
Default CliDescriptionAttribute is under consideration.

## Usage
CliNameAttribute isn't necessary.  
No specific coding convention is necessary.  
  
The following will interpret the command-line arguments, execute any underlying API, and return the result.
```csharp
var result = new CommandLineInterface.Execute(new Classlib());
```
It's also possible to simply populate any object.
```csharp
var settings = new Settings();
OptionDeserializer.Deserialize(settings)
```
To enable descriptions based on XML documentation, add the following to the project file.
```
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
### Demo Example
[https://github.com/calq-framework/cli/tree/main/Cli/Example](https://github.com/calq-framework/cli/tree/main/Cli/Example)

### Quick Start
```csharp
using CalqFramework.Cli;
using CalqFramework.Cli.Serialization;
using System.Text.Json;

var result = new CommandLineInterface().Execute(new QuickStart());
if (result != null)  Console.WriteLine(JsonSerializer.Serialize(result));

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
