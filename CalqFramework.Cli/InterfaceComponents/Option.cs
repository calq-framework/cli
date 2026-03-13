namespace CalqFramework.Cli.InterfaceComponents;

/// <summary>
///     Represents a CLI option (field or property) with its metadata.
/// </summary>
public sealed class Option {
    public required bool IsMultiValue { get; init; }
    public required IReadOnlyList<string> Keys { get; init; }
    public required MemberInfo MemberInfo { get; init; }
    public required Type ValueType { get; init; }
    public string? Value { get; init; }
}
