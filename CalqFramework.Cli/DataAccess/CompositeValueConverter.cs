using CalqFramework.DataAccess;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.Cli.DataAccess;

/// <summary>
///     Decorator that adds multi-value handling to a base value converter.
///     Uses composition to delegate simple type conversions while handling enumerables specially.
/// </summary>
/// <remarks>
///     Creates a new composite value converter that wraps a base converter.
/// </remarks>
/// <param name="baseConverter">The converter to use for simple types and enumerable elements.</param>
/// <param name="collectionStoreFactory">Factory for creating enumerable stores.</param>
public sealed class CompositeValueConverter(IValueConverter<string?> baseConverter, ICollectionElementStoreFactory<string, object?> collectionStoreFactory) : ICompositeValueConverter<string?> {
    private readonly IValueConverter<string?> _baseConverter = baseConverter;
    private readonly ICollectionElementStoreFactory<string, object?> _collectionElementStoreFactory = collectionStoreFactory;

    public bool CanConvert(Type targetType) => _baseConverter.CanConvert(targetType) || (IsMultiValue(targetType) && _baseConverter.CanConvert(targetType.GetEnumerableElementType()));

    public string? ConvertFrom(object? value, Type targetType) {
        if (IsMultiValue(targetType)) {
            if (value == null) {
                return null;
            }

            return ConvertEnumerableToString((IEnumerable)value, targetType);
        }

        return _baseConverter.ConvertFrom(value, targetType);
    }

    public object? ConvertToOrUpdate(string? value, Type targetType, object? currentValue) {
        if (IsMultiValue(targetType)) {
            return AppendToEnumerable(value, targetType, (IEnumerable)currentValue!);
        }

        return _baseConverter.ConvertToOrUpdate(value, targetType, currentValue);
    }

    public bool IsMultiValue(Type type) => !_baseConverter.CanConvert(type) && type.IsEnumerable();

    public Type GetValueType(Type type) => type.GetEnumerableElementType();

    private string? ConvertEnumerableToString(IEnumerable enumerable, Type targetType) {
        Type elementType = targetType.GetEnumerableElementType();

        IEnumerable<string?> elements = enumerable.Cast<object?>()
            .Select(item => _baseConverter.ConvertFrom(item, elementType));
        return "[" + string.Join(", ", elements) + "]";
    }

    private object? AppendToEnumerable(string? value, Type targetType, IEnumerable enumerable) {
        Type elementType = targetType.GetEnumerableElementType();

        object? item = _baseConverter.ConvertToOrUpdate(value, elementType, null);
        ICollectionElementStore<string, object?> store = _collectionElementStoreFactory.CreateStore(enumerable);

        return store.Append(item);
    }
}
