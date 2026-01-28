namespace CalqFramework.DataAccess.Parsing;

/// <summary>
/// Parses string values to typed objects.
/// </summary>
public class ValueParser : IValueParser {

    public bool IsParseable(Type type) {
        return type.IsPrimitive || type == typeof(string);
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
            _ => throw new ArgumentException($"type cannot be parsed: {targetType.Name}"),
        };
        
        return objValue;
    }
}
