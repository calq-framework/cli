using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {

    /// <summary>
    /// Provides mediated access to a key-value store through accessors (e.g., FieldInfo, PropertyInfo) with internal value conversion.
    /// </summary>
    public interface IMediatedKeyValueStore<TKey, TAccessor, TInternalValue> {
        /// <summary>
        /// Gets all available accessors (e.g., fields, properties, methods) in the store.
        /// </summary>
        protected internal IEnumerable<TAccessor> Accessors { get; }

        /// <summary>
        /// Gets or sets the internal value using the specified accessor.
        /// </summary>
        protected internal TInternalValue this[TAccessor accessor] { get; set; }

        /// <summary>
        /// Determines whether the store contains the specified accessor.
        /// </summary>
        protected internal bool ContainsAccessor(TAccessor accessor);

        /// <summary>
        /// Gets the accessor associated with the specified key.
        /// </summary>
        protected internal TAccessor GetAccessor(TKey key);

        /// <summary>
        /// Gets the data type of the value associated with the specified accessor.
        /// </summary>
        protected internal Type GetDataType(TAccessor accessor);

        /// <summary>
        /// Gets the internal value for the specified accessor, or initializes it if not set.
        /// </summary>
        protected internal TInternalValue GetValueOrInitialize(TAccessor accessor);

        /// <summary>
        /// Attempts to get the accessor for the specified key.
        /// </summary>
        protected internal bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TAccessor result);
    }
}
