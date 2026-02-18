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
        private readonly ICollectionElementStoreFactory<string, object?> _collectionStoreFactory;

        /// <summary>
        /// Creates a new composite value converter that wraps a base converter.
        /// </summary>
        /// <param name="baseConverter">The converter to use for simple types and collection elements.</param>
        /// <param name="collectionStoreFactory">Factory for creating collection stores.</param>
        public CompositeValueConverter(
            IValueConverter<string?> baseConverter,
            ICollectionElementStoreFactory<string, object?> collectionStoreFactory) {
            _baseConverter = baseConverter;
            _collectionStoreFactory = collectionStoreFactory;
        }

        public bool IsConvertible(Type type) {
            return _baseConverter.IsConvertible(type) || (type.IsCollection() && _baseConverter.IsConvertible(type.GetCollectionElementType()));
        }

        public string? ConvertFromInternalValue(object? value, Type internalType) {
            if (internalType.IsCollection()) {
                if (value == null) {
                    return null;
                }
                return ConvertCollectionToString((ICollection)value, internalType);
            }
            return _baseConverter.ConvertFromInternalValue(value, internalType);
        }

        public object? ConvertToInternalValue(string? value, Type internalType, object? currentValue) {
            if (internalType.IsCollection()) {
                AddStringToCollection(value, internalType, (ICollection)currentValue!);
                return currentValue;
            }
            return _baseConverter.ConvertToInternalValue(value, internalType, currentValue);
        }

        private string? ConvertCollectionToString(ICollection collection, Type internalType) {
            var elementType = internalType.GetCollectionElementType();
            
            var elements = collection.Cast<object?>().Select(item => _baseConverter.ConvertFromInternalValue(item, elementType));
            return "[" + string.Join(", ", elements) + "]";
        }

        private void AddStringToCollection(string? value, Type internalType, ICollection collection) {
            var elementType = internalType.GetCollectionElementType();
            
            var item = _baseConverter.ConvertToInternalValue(value, elementType, null);
            _collectionStoreFactory.CreateStore(collection).Add(item);
        }

        public bool IsCollection(Type type) {
            return type.IsCollection();
        }

        public Type GetDataType(Type type) {
            return type.GetCollectionElementType();
        }
    }
}
