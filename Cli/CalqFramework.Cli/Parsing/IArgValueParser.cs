using System;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Interface for parsing string values to typed objects.
/// </summary>
public interface IArgValueParser {
    /// <summary>
    /// Determines if a type can be parsed from a string.
    /// </summary>
    bool IsParsable(Type type);

    /// <summary>
    /// Parses a string value to the specified type.
    /// </summary>
    T Parse<T>(string value);

    /// <summary>
    /// Parses a string value to the specified type using a specific format provider.
    /// </summary>
    T Parse<T>(string value, IFormatProvider? formatProvider);

    /// <summary>
    /// Parses a string value to the specified target type.
    /// </summary>
    object Parse(string value, Type targetType);

    /// <summary>
    /// Parses a string value to the specified target type using a specific format provider.
    /// </summary>
    object Parse(string value, Type targetType, IFormatProvider? formatProvider);
}
