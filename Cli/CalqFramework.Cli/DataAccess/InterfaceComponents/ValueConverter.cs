using System;
using System.Collections;
using System.Linq;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public class ValueConverter : IValueConverter<string?> {

        private readonly ICollectionStoreFactory<string, object?> _collectionStoreFactory;
        private readonly IStringParser _argValueParser;

        public ValueConverter(ICollectionStoreFactory<string, object?> collectionStoreFactory, IStringParser argValueParser) {
            _collectionStoreFactory = collectionStoreFactory;
            _argValueParser = argValueParser;
        }

        public string? ConvertFromInternalValue(object? value, Type internalType) {
            if (value == null) {
                return null;
            }

            bool isCollection = internalType.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                return value.ToString();
            } else {
                ICollection collection = (value as ICollection)!;
                return "[" + string.Join(", ", collection.Cast<object?>().Select(x => x?.ToString() ?? "NULL").Cast<string?>()) + "]";
            }
        }

        public object? ConvertToInternalValue(string? value, Type internalType, object? currentValue) {
            if (value == null) {
                return null;
            }

            bool isCollection = internalType.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                return _argValueParser.ParseValue(value, internalType);
            } else {
                ICollection collection = (currentValue as ICollection)!;
                object item = _argValueParser.ParseValue(value, internalType.GetGenericArguments()[0]);
                _collectionStoreFactory.CreateStore(collection).Add(item);
                return currentValue;
            }
        }
    }
}
