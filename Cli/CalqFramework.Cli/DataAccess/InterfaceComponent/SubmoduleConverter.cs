using CalqFramework.Cli.DataAccess.ClassMember;
using System;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class SubmoduleConverter : IValueConverter<object?> {

        public object? ConvertFromInternalValue(object? value, Type internalType) {
            return value;
        }

        public object? ConvertToInternalValue(object? value, Type internalType) {
            return value;
        }
    }
}