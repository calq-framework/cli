using System;
using System.Collections;
using System.Linq;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Parsing;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class OptionConverter : IValueConverter<string?> {

        public string? ConvertFromInternalValue(object? value, Type internalType) {
            if (value == null) {
                return "NULL";
            }

            bool isCollection = internalType.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                return value.ToString();
            } else {
                var collection = (value as ICollection)!;
                return string.Join(", ", collection.Cast<object?>().Select(x => x?.ToString() ?? "NULL").Cast<string?>());
            }
        }

        public object? ConvertToInternalValue(string? value, Type internalType, object? currentValue) {
            if (value == null) {
                return value;
            }

            bool isCollection = internalType.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                return ValueParser.ParseValue(value, internalType);
            } else {
                var collection = (currentValue as ICollection)!;
                var item = ValueParser.ParseValue(value, internalType.GetGenericArguments()[0]);
                new CollectionStore(collection).AddValue(item);
                return currentValue;
            }
        }
    }
}
