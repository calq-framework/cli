using System;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public interface IValueConverter<TValue> {
        /// <summary>
        /// Converts an internal value to the CLI representation.
        /// </summary>
        TValue ConvertFromInternalValue(object? value, Type internalType);

        /// <summary>
        /// Converts a CLI value to the internal object representation.
        /// </summary>
        object? ConvertToInternalValue(TValue value, Type internalType, object? currentValue);
    }
}
