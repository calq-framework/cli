# calq options
Calq Options is a CLI option deserializer that is compliant with GNU (and POSIX) [conventions](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html).

## Get Started
```csharp
class HelloWorld {
    [NameAttribute("world")] // would be "helloWorld" by default
    [ShortNameAttribute('w')] // would be 'h' by default
    public bool helloWorld;
}
````
```csharp
Opts.Load(new HelloWorld())
````
