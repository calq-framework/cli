using System;
using System.Collections;
using System.Linq;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.Text;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public class ValueConverter : IValueConverter<string?> {

        private readonly ICollectionStoreFactory _collectionStoreFactory;
        private readonly Parsing.ValueParser _valueParser;

        public ValueConverter(ICollectionStoreFactory collectionStoreFactory, Parsing.ValueParser valueParser) {
            _collectionStoreFactory = collectionStoreFactory;
            _valueParser = valueParser;
        }

        /// <summary>
        /// Gets the value parser used by this converter.
        /// </summary>
        public Parsing.ValueParser ValueParser => _valueParser;

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
                return _valueParser.ParseValue(value, internalType);
            } else {
                ICollection collection = (currentValue as ICollection)!;
                object item = _valueParser.ParseValue(value, internalType.GetGenericArguments()[0]);
                _collectionStoreFactory.CreateStore(collection).AddValue(item);
                return currentValue;
            }
        }
    }
}
