using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Parsing;
using System;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class OptionConverter : IValueConverter<string?> {
        public string? ConvertFromInternalValue(object? value, Type internalType) {
            return value?.ToString()?.ToLower();
        }

        public object? ConvertToInternalValue(string? value, Type internalType) {
            return ValueParser.ParseValue(value, internalType);
        }
    }
}