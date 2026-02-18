using System;

namespace CalqFramework.Cli.DataAccess {
    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public interface IValueConverter<TValue> {
        /// <summary>
        /// Determines if a type can be converted from CLI representation.
        /// </summary>
        bool CanConvert(Type targetType);

        /// <summary>
        /// Converts an internal value to the CLI representation.
        /// </summary>
        TValue ConvertFrom(object? value, Type targetType);

        /// <summary>
        /// Converts a CLI value to the internal object representation, or updates an existing value.
        /// </summary>
        object? ConvertToOrUpdate(TValue value, Type targetType, object? currentValue);
    }
}
