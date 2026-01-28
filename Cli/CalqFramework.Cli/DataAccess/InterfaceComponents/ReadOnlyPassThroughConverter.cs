using System;
using CalqFramework.Cli.DataAccess.ClassMembers;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    internal class ReadOnlyPassThroughConverter : IValueConverter<object?> {

        public object? ConvertFromInternalValue(object? value, Type internalType) {
            return value;
        }

        public object? ConvertToInternalValue(object? value, Type internalType, object? currentValue) => throw new NotSupportedException();
    }
}
