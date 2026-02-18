using System;

namespace CalqFramework.Cli.DataAccess {
    /// <summary>
    /// Extends IValueConverter with collection metadata capabilities.
    /// Provides information about whether a type is handled as a collection and its element type.
    /// </summary>
    public interface ICompositeValueConverter<TValue> : IValueConverter<TValue> {
        /// <summary>
        /// Determines if the specified type is handled as a collection by this converter.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is handled as a collection; otherwise, false.</returns>
        bool IsCollection(Type type);

        /// <summary>
        /// Gets the data type for the specified type.
        /// For collection types, returns the element type; for non-collection types, returns the type itself.
        /// </summary>
        /// <param name="type">The type to get data type for.</param>
        /// <returns>The element type for collections, or the type itself for non-collections.</returns>
        Type GetDataType(Type type);
    }
}
