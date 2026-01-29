using System;
using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Parses string values to typed objects in CLI context with enhanced error messages.
/// </summary>
public class ArgValueParser : IStringParser {
    private readonly StringParser _stringParser = new();

    public bool IsParsable(Type type) {
        return _stringParser.IsParsable(type) || type.GetInterface(nameof(ICollection)) != null;
    }

    public T ParseValue<T>(string value) {
        return (T)ParseValue(value, typeof(T));
    }

    public object ParseValue(string value, Type type) {
        bool isCollection = type.GetInterface(nameof(ICollection)) != null;
        if (isCollection) {
            type = type.GetGenericArguments()[0];
        }

        try {
            return _stringParser.ParseValue(value, type);
        } catch (OverflowException ex) {
            var minField = type.GetField("MinValue");
            var maxField = type.GetField("MaxValue");
            
            if (minField != null && maxField != null) {
                var min = minField.GetValue(null);
                var max = maxField.GetValue(null);
                throw new ArgValueParserException($"out of range ({min}-{max})", ex);
            }
            throw;
        } catch (FormatException ex) {
            throw new ArgValueParserException($"invalid format (expected {type.Name})", ex);
        }
    }
}
