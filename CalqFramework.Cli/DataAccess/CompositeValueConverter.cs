using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.CollectionElementStores;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Decorator that adds multi-value handling to a base value converter.
    /// Uses composition to delegate simple type conversions while handling enumerables specially.
    /// </summary>
    public sealed class CompositeValueConverter : ICompositeValueConverter<string?> {

        private readonly IValueConverter<string?> _baseConverter;
        private readonly ICollectionElementStoreFactory<string, object?> _collectionElementStoreFactory;

        /// <summary>
        /// Creates a new composite value converter that wraps a base converter.
        /// </summary>
        /// <param name="baseConverter">The converter to use for simple types and enumerable elements.</param>
        /// <param name="collectionStoreFactory">Factory for creating enumerable stores.</param>
        public CompositeValueConverter(
            IValueConverter<string?> baseConverter,
            ICollectionElementStoreFactory<string, object?> collectionStoreFactory) {
            _baseConverter = baseConverter;
            _collectionElementStoreFactory = collectionStoreFactory;
        }

        public bool CanConvert(Type targetType) {
            return _baseConverter.CanConvert(targetType) || (IsMultiValue(targetType) && _baseConverter.CanConvert(targetType.GetEnumerableElementType()));
        }

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

        private string? ConvertEnumerableToString(IEnumerable enumerable, Type targetType) {
            var elementType = targetType.GetEnumerableElementType();
            
            var elements = enumerable.Cast<object?>().Select(item => _baseConverter.ConvertFrom(item, elementType));
            return "[" + string.Join(", ", elements) + "]";
        }

        private object? AppendToEnumerable(string? value, Type targetType, IEnumerable enumerable) {
            var elementType = targetType.GetEnumerableElementType();
            
            var item = _baseConverter.ConvertToOrUpdate(value, elementType, null);
            var store = _collectionElementStoreFactory.CreateStore(enumerable);
            
            return store.Append(item);
        }

        public bool IsMultiValue(Type type) {
            return !_baseConverter.CanConvert(type) && type.IsEnumerable();
        }

        public Type GetValueType(Type type) {
            return type.GetEnumerableElementType();
        }
    }
}
