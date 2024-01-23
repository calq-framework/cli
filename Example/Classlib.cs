using CalqFramework.Cli.Attributes;

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
