using CalqFramework.DataAccess;

namespace CalqFramework.Extensions.System;

/// <summary>
/// Extension methods for parsing string values to typed objects.
/// </summary>
public static class TypeExtensions {

    /// <summary>
    /// Determines whether the specified type is a collection type.
    /// </summary>
    public static bool IsCollection(this global::System.Type type) {
        return type.GetInterface(nameof(global::System.Collections.ICollection)) != null;
    }

    /// <summary>
    /// Gets the element type for a collection, or the type itself if not a collection.
    /// For generic collections like List&lt;T&gt;, returns T. For non-collections, returns the type itself.
    /// </summary>
    public static global::System.Type GetCollectionElementType(this global::System.Type type) {
        if (!type.IsCollection()) {
            return type;
        }
        
        var genericArgs = type.GetGenericArguments();
        return genericArgs.Length > 0 ? genericArgs[0] : type;
    }

    public static bool IsParsable(this global::System.Type type) {
        type = Nullable.GetUnderlyingType(type) ?? type;
        
        switch (Type.GetTypeCode(type)) {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Char:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.DateTime:
            case TypeCode.String:
                return true;
            default:
                var parsableInterface = type.GetInterface("IParsable`1");
                return parsableInterface != null;
        }
    }

    public static T Parse<T>(this Type type, string value) {
        return (T)type.Parse(value);
    }

    public static T Parse<T>(this Type type, string value, IFormatProvider formatProvider) {
        return (T)type.Parse(value, formatProvider);
    }

    public static object Parse(this Type targetType, string value) {
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
            _ => TryParseParsable(targetType, value)
        };
        
        return objValue;
    }

    public static object Parse(this Type targetType, string value, IFormatProvider formatProvider) {
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
            _ => TryParseParsableWithFormatProvider(targetType, value, formatProvider)
        };
        
        return objValue;
    }

    private static object TryParseParsable(Type targetType, string value) {
        if (Nullable.GetUnderlyingType(targetType) != null) {
            targetType = Nullable.GetUnderlyingType(targetType)!;
            return Parse(targetType, value);
        }

        var parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
            var parseMethod = targetType.GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod != null) {
                try {
                    return parseMethod.Invoke(null, new object?[] { value })!;
                } catch (global::System.Reflection.TargetInvocationException ex) {
                    throw ex.InnerException ?? ex;
                }
            }
        }
        
        throw DataAccessErrors.TypeCannotBeParsed(targetType.Name);
    }

    private static object TryParseParsableWithFormatProvider(Type targetType, string value, IFormatProvider formatProvider) {
        if (Nullable.GetUnderlyingType(targetType) != null) {
            targetType = Nullable.GetUnderlyingType(targetType)!;
            return Parse(targetType, value, formatProvider);
        }

        var parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
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
        
        throw DataAccessErrors.TypeCannotBeParsed(targetType.Name);
    }
}
