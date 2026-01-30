namespace CalqFramework.Extensions.System;

/// <summary>
/// Extension methods for parsing string values to typed objects.
/// </summary>
public static class TypeExtensions {

    public static bool IsParsable(this Type type) {
        type = Nullable.GetUnderlyingType(type) ?? type;
        
        if (type.IsPrimitive || type == typeof(string)) {
            return true;
        }

        var parsableInterface = type.GetInterface("IParsable`1");
        return parsableInterface != null;
    }

    public static T Parse<T>(this Type type, string value) {
        return (T)type.Parse(value);
    }

    public static T Parse<T>(this Type type, string value, IFormatProvider? formatProvider) {
        return (T)type.Parse(value, formatProvider);
    }

    public static object Parse(this Type targetType, string value) {
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
            TypeCode.DateTime => DateTime.Parse(value),
            TypeCode.String => value,
            _ => TryParseParsable(targetType, value, null)
        };
        
        return objValue;
    }

    public static object Parse(this Type targetType, string value, IFormatProvider? formatProvider) {
        formatProvider ??= global::System.Globalization.CultureInfo.InvariantCulture;
        
        if (Nullable.GetUnderlyingType(targetType) != null) {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        object objValue = Type.GetTypeCode(targetType) switch {
            TypeCode.Boolean => bool.Parse(value),
            TypeCode.Byte => byte.Parse(value, formatProvider),
            TypeCode.SByte => sbyte.Parse(value, formatProvider),
            TypeCode.Char => char.Parse(value),
            TypeCode.Decimal => decimal.Parse(value, formatProvider),
            TypeCode.Double => double.Parse(value, formatProvider),
            TypeCode.Single => float.Parse(value, formatProvider),
            TypeCode.Int32 => int.Parse(value, formatProvider),
            TypeCode.UInt32 => uint.Parse(value, formatProvider),
            TypeCode.Int64 => long.Parse(value, formatProvider),
            TypeCode.UInt64 => ulong.Parse(value, formatProvider),
            TypeCode.Int16 => short.Parse(value, formatProvider),
            TypeCode.UInt16 => ushort.Parse(value, formatProvider),
            TypeCode.DateTime => DateTime.Parse(value, formatProvider),
            TypeCode.String => value,
            _ => TryParseParsable(targetType, value, formatProvider)
        };
        
        return objValue;
    }

    private static object TryParseParsable(Type targetType, string value, IFormatProvider? formatProvider) {
        var parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
            if (formatProvider == null) {
                var parseMethod = targetType.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod != null) {
                    try {
                        return parseMethod.Invoke(null, new object?[] { value })!;
                    } catch (global::System.Reflection.TargetInvocationException ex) {
                        throw ex.InnerException ?? ex;
                    }
                }
            } else {
                var parseMethod = targetType.GetMethod("Parse", 
                    new[] { typeof(string), typeof(IFormatProvider) });
                
                if (parseMethod != null) {
                    try {
                        return parseMethod.Invoke(null, new object?[] { value, formatProvider })!;
                    } catch (global::System.Reflection.TargetInvocationException ex) {
                        throw ex.InnerException ?? ex;
                    }
                }
            }
        }
        
        throw new ArgumentException($"type cannot be parsed: {targetType.Name}");
    }
}
