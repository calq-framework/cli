using System;

namespace CalqFramework.Cli.DataAccess {
    /// <summary>
    /// Extends IValueConverter with multi-value metadata capabilities.
    /// Provides information about whether a type is handled as a multi-value parameter and its element type.
    /// </summary>
    public interface ICompositeValueConverter<TValue> : IValueConverter<TValue> {
        /// <summary>
        /// Determines if the specified type is handled as a multi-value parameter by this converter.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is handled as a multi-value parameter; otherwise, false.</returns>
        bool IsMultiValue(Type type);

        /// <summary>
        /// Gets the data type for the specified type.
        /// For multi-value types, returns the element type; for single-value types, returns the type itself.
        /// </summary>
        /// <param name="type">The type to get data type for.</param>
        /// <returns>The element type for multi-value types, or the type itself for single-value types.</returns>
        Type GetValueType(Type type);
    }
}
