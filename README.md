# CalqFramework.Options
CalqFramework.Options helps to convert libraries into command-line tools compliant with GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html). CalqFramework.Options exposes class data members and methods as commands, enabling users to interact with class instances via auto-generated CLI.

## Features
- **Method invocation**: Executes class methods with regular and optional parameters, all at the same time treated as positional arguments.
- **Parameter types**: Supports primitive types (integer, boolean, double, etc.), strings, and collections.
- **GNU-style options**: Handles GNU-style options (e.g., `--text`, `--integer`) adhering to POSIX conventions for command-line arguments.
- **Simple value validation**: Ensures the correctness of parameter values with built-in validation based on parameter types.
- **Help information**: Obtains help information based on reflection, providing available commands, their parameters, and global options. (TODO: extract help information from documentation)

## Usage

### Example class library
```csharp
public class ExampleClassLibrary
{
    [ShortName("e")]
    public bool Enable { get; set; }
    
    [Name("verbose")]
    public bool VerboseMode { get; set; }

    public class Nested
    {
        public void Command1(string text)
        {
            Console.WriteLine($"Executing Command1 with text: {text}");
        }

        public void Command2(int number)
        {
            Console.WriteLine($"Executing Command2 with number: {number}");
        }

        public class DoubleNested
        {
            public void Command(string option)
            {
                Console.WriteLine($"Executing Command with option: {option}");
            }
        }
    }

    public void Option(string opt1, string opt2)
    {
        Console.WriteLine($"Executing Option with options: {opt1}, {opt2}");
    }

    public void Collection(List<int> numbers)
    {
        Console.WriteLine($"Executing Collection with numbers: {string.Join(", ", numbers)}");
    }

    public void Feature()
    {
        Console.WriteLine($"Executing Feature with Enable set to: {Enable}");
    }
    
    public void Verbose()
    {
        Console.WriteLine($"Executing Verbose with VerboseMode set to: {VerboseMode}");
    }
    
    public void Stacked(bool all = false, bool be = false, bool created = false)
    {
        Console.WriteLine($"Executing Stacked with options: {a}, {b}, {c}");
    }
}
```

### Example program
public class Example
{
    static void Main(string[] args) {
        CommandLineInterface.Execute(new ExampleClassLibrary(), args);
    }
}

### Example usage from command-line
```
example.exe --help
```
```
example.exe nested -h
```
```
example.exe nested command1 --text Hello
```
```
example.exe nested command2 --number 42
```
```
example.exe nested doublenested command --option abc
```
```
example.exe option --opt1 value1 --opt2 value2
```
```
example.exe option value1 value2
```
```
example.exe option -- --value1 -value2
```
```
example.exe collection --numbers 1 --numbers 2 --numbers 3
```
```
example.exe feature --enable
```
```
example.exe feature -e
```
```
example.exe verbose --verbose
```
```
example.exe stacked -abc
```

