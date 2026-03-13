namespace CalqFramework.Cli;

/// <summary>
///     Attribute to specify custom CLI names for class members (supports multiple aliases).
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class CliNameAttribute(string name) : Attribute {
    public string Name { get; } = name;
}
