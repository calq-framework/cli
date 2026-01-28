using System;
using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Parses string values to typed objects in CLI context with enhanced error messages.
/// </summary>
public class CliValueParser : IValueParser {
    private readonly ValueParser _valueParser = new();

    public bool IsParseable(Type type) {
        return type.IsPrimitive || type == typeof(string) || type.GetInterface(nameof(ICollection)) != null;
    }

    public T ParseValue<T>(string value) {
        return (T)ParseValue(value, typeof(T));
    }

    public object ParseValue(string value, Type type) {
        try {
            bool isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection) {
                type = type.GetGenericArguments()[0];
            }

            return _valueParser.ParseValue(value, type);
        } catch (OverflowException ex) {
            throw new CliException($"value is out of range: {ex.Message}", ex);
        } catch (FormatException ex) {
            throw new CliException($"value type mismatch: expected {type.Name} got {value}", ex);
        } catch (ArgumentException ex) {
            throw new CliException($"value type mismatch: expected {type.Name} got {value}", ex);
        }
    }
}
