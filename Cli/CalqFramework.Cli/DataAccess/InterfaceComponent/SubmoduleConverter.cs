using System;
using CalqFramework.Cli.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class SubmoduleConverter : IValueConverter<object?> {

        public object? ConvertFromInternalValue(object? value, Type internalType) {
            return value;
        }

        public object? ConvertToInternalValue(object? value, Type internalType, object? currentValue) => throw new NotSupportedException();
    }
}
