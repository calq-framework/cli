namespace CalqFramework.DataAccess.Extensions.System;

/// <summary>
///     Extension methods for parsing string values to typed objects.
/// </summary>
public static class TypeExtensions {
    private static readonly Dictionary<Type, Type> s_interfaceToConcreteMap = new() {
        // Generic collections
        [typeof(IList<>)] = typeof(List<>),
        [typeof(ICollection<>)] = typeof(List<>),
        [typeof(IEnumerable<>)] = typeof(List<>),
        [typeof(IReadOnlyList<>)] = typeof(List<>),
        [typeof(IReadOnlyCollection<>)] = typeof(List<>),
        [typeof(IDictionary<,>)] = typeof(Dictionary<,>),
        [typeof(IReadOnlyDictionary<,>)] = typeof(Dictionary<,>),
        [typeof(ISet<>)] = typeof(HashSet<>),
        [typeof(IReadOnlySet<>)] = typeof(HashSet<>),

        // Non-generic collections
        [typeof(IList)] = typeof(ArrayList),
        [typeof(ICollection)] = typeof(ArrayList),
        [typeof(IEnumerable)] = typeof(ArrayList),
        [typeof(IDictionary)] = typeof(Hashtable)
    };

    /// <summary>
    ///     Determines whether the specified type is an enumerable type.
    /// </summary>
    public static bool IsEnumerable(this Type type) => type.GetInterface(nameof(IEnumerable)) != null;

    /// <summary>
    ///     Gets the element type for an enumerable, or the type itself if not an enumerable.
    ///     For generic enumerables like List&lt;T&gt;, returns T. For non-enumerables, returns the type itself.
    /// </summary>
    public static Type GetEnumerableElementType(this Type type) {
        if (!type.IsEnumerable()) {
            return type;
        }

        Type[] genericArgs = type.GetGenericArguments();
        return genericArgs.Length > 0 ? genericArgs[0] : typeof(object);
    }

    /// <summary>
    ///     Creates an instance of the specified type, with support for collection interfaces.
    /// </summary>
    /// <param name="type">The type to instantiate.</param>
    /// <returns>A new instance of the type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type cannot be instantiated.</exception>
    public static object CreateInstance(this Type type) =>
        Type.GetTypeCode(type) switch {
            TypeCode.Boolean => false,
            TypeCode.Byte => (byte)0,
            TypeCode.SByte => (sbyte)0,
            TypeCode.Char => '\0',
            TypeCode.Decimal => 0m,
            TypeCode.Double => 0.0,
            TypeCode.Single => 0f,
            TypeCode.Int32 => 0,
            TypeCode.UInt32 => 0u,
            TypeCode.Int64 => 0L,
            TypeCode.UInt64 => 0ul,
            TypeCode.Int16 => (short)0,
            TypeCode.UInt16 => (ushort)0,
            TypeCode.DateTime => default(DateTime),
            TypeCode.String => string.Empty,
            _ => TryCreateInstance(type) ?? TryCreateInstance(Nullable.GetUnderlyingType(type)) ?? TryCreateInstance(GetConcreteCollectionType(type)) ?? throw DataAccessErrors.CannotCreateInstance(type.FullName ?? type.Name)
        };

    private static object? TryCreateInstance(Type? type) {
        if (type == null) {
            return null;
        }

        try {
            return Activator.CreateInstance(type);
        } catch {
            return null;
        }
    }

    private static Type? GetConcreteCollectionType(Type interfaceType) {
        if (s_interfaceToConcreteMap.TryGetValue(interfaceType, out Type? concreteType)) {
            return concreteType;
        }

        if (interfaceType.IsGenericType) {
            Type genericTypeDef = interfaceType.GetGenericTypeDefinition();
            if (s_interfaceToConcreteMap.TryGetValue(genericTypeDef, out Type? genericConcrete)) {
                return genericConcrete.MakeGenericType(interfaceType.GetGenericArguments());
            }
        }

        return null;
    }

    public static bool IsParsable(this Type type) {
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
                Type? parsableInterface = type.GetInterface("IParsable`1");
                return parsableInterface != null;
        }
    }

    public static T Parse<T>(this Type type, string value) => (T)type.Parse(value);

    public static T Parse<T>(this Type type, string value, IFormatProvider formatProvider) =>
        (T)type.Parse(value, formatProvider);

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
            return targetType.Parse(value);
        }

        Type? parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
            MethodInfo? parseMethod = targetType.GetMethod("Parse", [typeof(string)]);
            if (parseMethod != null) {
                try {
                    return parseMethod.Invoke(null, [value])!;
                } catch (TargetInvocationException ex) {
                    throw ex.InnerException ?? ex;
                }
            }
        }

        throw DataAccessErrors.TypeCannotBeParsed(targetType.Name);
    }

    private static object TryParseParsableWithFormatProvider(Type targetType, string value, IFormatProvider formatProvider) {
        if (Nullable.GetUnderlyingType(targetType) != null) {
            targetType = Nullable.GetUnderlyingType(targetType)!;
            return targetType.Parse(value, formatProvider);
        }

        Type? parsableInterface = targetType.GetInterface("IParsable`1");
        if (parsableInterface != null) {
            MethodInfo? parseMethod = targetType.GetMethod("Parse", [typeof(string), typeof(IFormatProvider)]);

            if (parseMethod != null) {
                try {
                    return parseMethod.Invoke(null, [value, formatProvider])!;
                } catch (TargetInvocationException ex) {
                    throw ex.InnerException ?? ex;
                }
            }
        }

        throw DataAccessErrors.TypeCannotBeParsed(targetType.Name);
    }
}
