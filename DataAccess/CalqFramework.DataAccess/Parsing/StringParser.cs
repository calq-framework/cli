namespace CalqFramework.DataAccess.Parsing;

/// <summary>
/// Parses string values to typed objects.
/// </summary>
public class StringParser : IStringParser {

    public bool IsParsable(Type type) {
        type = Nullable.GetUnderlyingType(type) ?? type;
        
        if (type.IsPrimitive || type == typeof(string)) {
            return true;
        }

        var parsableInterface = type.GetInterface("IParsable`1");
        return parsableInterface != null;
    }

    public T ParseValue<T>(string value) {
        return (T)ParseValue(value, typeof(T));
    }

    public object ParseValue(string value, Type targetType) {
        if (Nullable.GetUnderlyingType(targetType) != null) {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        object objValue = Type.GetTypeCode(targetType) switch {
            TypeCode.Boolean => bool.Parse(value),
            TypeCode.Byte => byte.Parse(value),
            TypeCode.SByte => sbyte.Parse(value),
            TypeCode.Char => char.Parse(value),
            TypeCode.Decimal => decimal.Parse(value),
            TypeCode.Double => double.Parse(value),
            TypeCode.Single => float.Parse(value),
            TypeCode.Int32 => int.Parse(value),
            TypeCode.UInt32 => uint.Parse(value),
            TypeCode.Int64 => long.Parse(value),
            TypeCode.UInt64 => ulong.Parse(value),
            TypeCode.Int16 => short.Parse(value),
            TypeCode.UInt16 => ushort.Parse(value),
            TypeCode.String => value,
            _ => TryParseParsable(value, targetType)
        };
        
        return objValue;
    }

    private static object TryParseParsable(string value, Type targetType) {
        // Check if type implements IParsable<T>
        var parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
            // Get the Parse method from the actual type, not the interface
            var parseMethod = targetType.GetMethod("Parse", 
                new[] { typeof(string), typeof(IFormatProvider) });
            
            if (parseMethod != null) {
                try {
                    return parseMethod.Invoke(null, new object?[] { value, null })!;
                } catch (System.Reflection.TargetInvocationException ex) {
                    // Unwrap the inner exception for cleaner error messages
                    throw ex.InnerException ?? ex;
                }
            }
        }
        
        throw new ArgumentException($"type cannot be parsed: {targetType.Name}");
    }
}
