# CalqFramework.Cli
CalqFramework.Cli helps to convert libraries into command-line tools compliant with GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html). CalqFramework.Cli exposes class data members and methods as commands, enabling users to interact with class instances via auto-generated CLI.

## Features
- **Method invocation**: Executes class methods with regular and optional parameters, both treated also as positional arguments.
- **Parameter types**: Supports primitive types (integer, boolean, double, etc.), strings, and collections.
- **GNU-style options**: Handles GNU-style options (e.g., ```--text```, ```--integer```) adhering to POSIX conventions for command-line arguments.
- **Value validation**: Ensures the correctness of parameter values with validation based on parameter types.
- **Help information**: Obtains help information based on reflection, providing available commands, their parameters, and global options. (TODO: extract help information from documentation)

## Usage

### Example tool
```csharp
CommandLineInterface.Execute(new Classlib());
```

### Example class library
```csharp
public class Classlib {
    [ShortName('x')] // default 'e'
    public bool Enable { get; set; }

    [Name("verbose")] // default "VerboseMode"
    public bool VerboseMode { get; set; }

    public Nested Nested { get; } = new();

    public void Option(string opt1, string opt2) {
        Console.WriteLine($"Option with options: {opt1}, {opt2}");
    }

    public void Collection(List<int> numbers) {
        Console.WriteLine($"Collection with numbers: {string.Join(", ", numbers)}");
    }

    public void Feature() {
        Console.WriteLine($"Feature with Enable set to: {Enable}");
    }

    public void Verbose() {
        Console.WriteLine($"Verbose with VerboseMode set to: {VerboseMode}");
    }

    public void Stacked(bool all = false, bool be = false, bool created = false) {
        Console.WriteLine($"Stacked with options: {all}, {be}, {created}");
    }
}

public class Nested {

    public DoubleNested DoubleNested { get; } = new();

    public void Command1(string text) {
        Console.WriteLine($"Command1 with text: {text}");
    }

    public void Command2(int number) {
        Console.WriteLine($"Command2 with number: {number}");
    }
}

public class DoubleNested {
    public void Command(string option) {
        Console.WriteLine($"Command with option: {option}");
    }
}

```

### Example usage from command-line

```
dotnet Example.dll --help
```
>CORE COMMANDS:<br/>
>Nested<br/>
><br/>
>ACTION COMMANDS:<br/>
>Option(String opt1, String opt2)<br/>
>Collection(List```1 numbers)<br/>
>Feature()<br/>
>Verbose()<br/>
>Stacked(Boolean all = False, Boolean be = False, Boolean created = False)<br/>
><br/>
>GLOBAL OPTIONS:<br/>
>--Enable, -x [Boolean: False]<br/>
>--verbose, -V [Boolean: False]<br/>
```
dotnet Example.dll nested --help
```
>CORE COMMANDS:<br/>
>DoubleNested<br/>
><br/>
>ACTION COMMANDS:<br/>
>Command1(String text)<br/>
>Command2(Int32 number)<br/>
><br/>
>GLOBAL OPTIONS:<br/>
```
dotnet Example.dll nested command1 --help
```
>--text, -t - Type: System.String, Required: True, Default: N/A
```
dotnet Example.dll nested command1 --text Hello
```
>Command1 with text: Hello
```
dotnet Example.dll nested command2 --number 42
```
>Command2 with number: 42
```
dotnet Example.dll nested doublenested command --option abc
```
>Command with option: abc
```
dotnet Example.dll option --opt1 value1 --opt2 value2
```
>Option with options: value1, value2
```
dotnet Example.dll option value1 value2
```
>Option with options: value1, value2
```
dotnet Example.dll option -- --value1 -value2
```
>Option with options: --value1, -value2
```
dotnet Example.dll collection --numbers 1 --numbers 2 --numbers 3
```
>Collection with numbers: 1, 2, 3
```
dotnet Example.dll feature --enable
```
>Feature with Enable set to: True
```
dotnet Example.dll feature -x
```
>Feature with Enable set to: True
```
dotnet Example.dll verbose --verbose
```
>Verbose with VerboseMode set to: True
```
dotnet Example.dll stacked -abc
```
>Stacked with options: True, True, True
```
dotnet Example.dll stacked +abc
```
>Stacked with options: False, False, False
