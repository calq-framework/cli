namespace CalqFramework.DataAccess.Parsing;

/// <summary>
/// Interface for parsing string values to typed objects.
/// </summary>
public interface IStringParser {
    /// <summary>
    /// Determines if a type can be parsed from a string.
    /// </summary>
    bool IsParsable(Type type);

    /// <summary>
    /// Parses a string value to the specified type.
    /// </summary>
    T ParseValue<T>(string value);

    /// <summary>
    /// Parses a string value to the specified type using a specific format provider.
    /// </summary>
    T ParseValue<T>(string value, IFormatProvider? formatProvider);

    /// <summary>
    /// Parses a string value to the specified target type.
    /// </summary>
    object ParseValue(string value, Type targetType);

    /// <summary>
    /// Parses a string value to the specified target type using a specific format provider.
    /// </summary>
    object ParseValue(string value, Type targetType, IFormatProvider? formatProvider);
}
