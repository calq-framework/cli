namespace CalqFramework.DataAccess;

/// <summary>
///     Factory class for creating common DataAccessException instances with consistent messaging.
/// </summary>
public static class DataAccessErrors {
    /// <summary>
    ///     Creates an exception for when a key is not found in a store.
    /// </summary>
    public static DataAccessException KeyNotFound<TKey>(TKey key) =>
        new($"Key '{key}' not found.");

    /// <summary>
    ///     Creates an exception for ambiguous keys in dual key stores.
    /// </summary>
    public static DataAccessException AmbiguousKey(object? key) =>
        new($"Ambiguous key '{key}', enable shadowing to ignore this error");

    /// <summary>
    ///     Creates an exception for unexpected arguments in method execution.
    /// </summary>
    public static DataAccessException UnexpectedArgument(object? argument) =>
        new($"Unexpected argument: {argument}");

    /// <summary>
    ///     Creates an exception for unassigned required parameters.
    /// </summary>
    public static DataAccessException UnassignedParameter(string parameterName) =>
        new($"Unassigned parameter: {parameterName}");

    /// <summary>
    ///     Creates an exception for when neither fields nor properties access is configured.
    /// </summary>
    public static ArgumentException NoAccessConfigured() =>
        new("Neither AccessFields nor AccessProperties is set");

    /// <summary>
    ///     Creates an exception for invalid key types in list operations.
    /// </summary>
    public static ArgumentException InvalidListKey(string? keyTypeName) =>
        new($"Key must be an integer or parsable string for list removal, got {keyTypeName ?? "null"}");

    /// <summary>
    ///     Creates an exception for unparsable types.
    /// </summary>
    public static ArgumentException TypeCannotBeParsed(string typeName) =>
        new($"Type cannot be parsed: {typeName}");

    /// <summary>
    ///     Creates an exception for unrecognized member types.
    /// </summary>
    public static ArgumentException UnrecognizedMemberType() =>
        new("MemberInfo is not a recognized type");

    /// <summary>
    ///     Creates an exception for unsupported operations.
    /// </summary>
    public static NotSupportedException OperationNotSupported(string operation) =>
        new($"Operation '{operation}' is not supported");

    /// <summary>
    ///     Creates an exception for when a type cannot be instantiated.
    /// </summary>
    public static InvalidOperationException CannotCreateInstance(string typeName) =>
        new($"Cannot create instance of type '{typeName}'");
}
