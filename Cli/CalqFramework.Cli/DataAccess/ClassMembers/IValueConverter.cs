using System;

namespace CalqFramework.Cli.DataAccess.ClassMembers {
    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public interface IValueConverter<TValue> {
        /// <summary>
        /// Determines if a type can be converted from CLI representation.
        /// </summary>
        bool IsConvertible(Type type);

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
