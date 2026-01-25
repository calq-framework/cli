namespace CalqFramework.DataAccess {

    /// <summary>
    /// Provides read-write access to a key-value store for accessing class members.
    /// </summary>
    public interface IKeyValueStore<TKey, TValue> : IReadOnlyKeyValueStore<TKey, TValue> {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        new TValue this[TKey key] { get; set; }

        /// <summary>
        /// Gets the value for the specified key, or initializes it if not set.
        /// </summary>
        TValue GetValueOrInitialize(TKey key);
    }

    /// <summary>
    /// Provides key-value store with mediated access through accessors and internal value conversion.
    /// </summary>
    public interface IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> : IKeyValueStore<TKey, TValue>, IMediatedKeyValueStore<TKey, TAccessor, TInternalValue> {
    }

    /// <summary>
    /// Provides key-value store with mediated access where internal value type matches external value type.
    /// </summary>
    public interface IKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue, TAccessor, TValue> {
    }
}
