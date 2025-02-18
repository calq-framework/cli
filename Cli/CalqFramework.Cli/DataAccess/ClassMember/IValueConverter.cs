using System;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface IValueConverter<TValue> {
        TValue ConvertFromInternalValue(object? value, Type internalType);

        object? ConvertToInternalValue(TValue value, Type internalType, object? currentValue);
    }
}
