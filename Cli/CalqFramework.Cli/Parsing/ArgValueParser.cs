using System;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Parses string values to typed objects with enhanced error messages.
/// </summary>
public class ArgValueParser : IArgValueParser {

    public bool IsParsable(Type type) {
        return type.IsParsable();
    }

    public T Parse<T>(string value) {
        return (T)Parse(value, typeof(T));
    }

    public T Parse<T>(string value, IFormatProvider? formatProvider) {
        return (T)Parse(value, typeof(T), formatProvider);
    }

    public object Parse(string value, Type type) {
        try {
            return type.Parse(value);
        } catch (OverflowException ex) {
            var minField = type.GetField("MinValue");
            var maxField = type.GetField("MaxValue");
            
            if (minField != null && maxField != null) {
                var min = minField.GetValue(null);
                var max = maxField.GetValue(null);
                throw ArgValueParserErrors.OutOfRange(min, max, ex);
            }
            throw;
        } catch (FormatException ex) {
            throw ArgValueParserErrors.InvalidFormat(type.Name, ex);
        }
    }

    public object Parse(string value, Type type, IFormatProvider? formatProvider) {
        try {
            return type.Parse(value, formatProvider);
        } catch (OverflowException ex) {
            var minField = type.GetField("MinValue");
            var maxField = type.GetField("MaxValue");
            
            if (minField != null && maxField != null) {
                var min = minField.GetValue(null);
                var max = maxField.GetValue(null);
                throw ArgValueParserErrors.OutOfRange(min, max, ex);
            }
            throw;
        } catch (FormatException ex) {
            throw ArgValueParserErrors.InvalidFormat(type.Name, ex);
        }
    }
}

