using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.CollectionElementStores;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Decorator that adds collection handling to a base value converter.
    /// Uses composition to delegate simple type conversions while handling collections specially.
    /// </summary>
    public sealed class CompositeValueConverter : ICompositeValueConverter<string?> {

        private readonly IValueConverter<string?> _baseConverter;
        private readonly ICollectionElementStoreFactory<string, object?> _collectionElementStoreFactory;

        /// <summary>
        /// Creates a new composite value converter that wraps a base converter.
        /// </summary>
        /// <param name="baseConverter">The converter to use for simple types and collection elements.</param>
        /// <param name="collectionStoreFactory">Factory for creating collection stores.</param>
        public CompositeValueConverter(
            IValueConverter<string?> baseConverter,
            ICollectionElementStoreFactory<string, object?> collectionStoreFactory) {
            _baseConverter = baseConverter;
            _collectionElementStoreFactory = collectionStoreFactory;
        }

        public bool CanConvert(Type targetType) {
            return _baseConverter.CanConvert(targetType) || (targetType.IsCollection() && _baseConverter.CanConvert(targetType.GetCollectionElementType()));
        }

        public string? ConvertFrom(object? value, Type targetType) {
            if (targetType.IsCollection()) {
                if (value == null) {
                    return null;
                }
                return ConvertCollectionToString((ICollection)value, targetType);
            }
            return _baseConverter.ConvertFrom(value, targetType);
        }

        public object? ConvertToOrUpdate(string? value, Type targetType, object? currentValue) {
            if (targetType.IsCollection()) {
                AddStringToCollection(value, targetType, (ICollection)currentValue!);
                return currentValue;
            }
            return _baseConverter.ConvertToOrUpdate(value, targetType, currentValue);
        }

        private string? ConvertCollectionToString(ICollection collection, Type targetType) {
            var elementType = targetType.GetCollectionElementType();
            
            var elements = collection.Cast<object?>().Select(item => _baseConverter.ConvertFrom(item, elementType));
            return "[" + string.Join(", ", elements) + "]";
        }

        private void AddStringToCollection(string? value, Type targetType, ICollection collection) {
            var elementType = targetType.GetCollectionElementType();
            
            var item = _baseConverter.ConvertToOrUpdate(value, elementType, null);
            _collectionElementStoreFactory.CreateStore(collection).Add(item);
        }

        public bool IsCollection(Type type) {
            return type.IsCollection();
        }

        public Type GetDataType(Type type) {
            return type.GetCollectionElementType();
        }
    }
}
