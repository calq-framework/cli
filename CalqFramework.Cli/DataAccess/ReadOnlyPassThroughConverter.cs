using System;
using CalqFramework.DataAccess.Extensions.System;

namespace CalqFramework.Cli.DataAccess;

internal sealed class ReadOnlyPassThroughConverter<TValue> : ICompositeValueConverter<TValue> {
    public bool CanConvert(Type targetType) => true;

    public TValue ConvertFrom(object? value, Type targetType) => (TValue)value!;

    public object? ConvertToOrUpdate(TValue value, Type targetType, object? currentValue) =>
        throw new NotSupportedException();

    public bool IsMultiValue(Type type) => type.IsEnumerable();

    public Type GetValueType(Type type) => type.GetEnumerableElementType();
}
