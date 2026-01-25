namespace CalqFramework.DataAccess {

    /// <summary>
    /// Provides read-only access to a key-value store for retrieving values from class members.
    /// </summary>
    public interface IReadOnlyKeyValueStore<TKey, TValue> {
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Determines whether the store contains the specified key.
        /// </summary>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Gets the data type of the value associated with the specified key.
        /// </summary>
        Type GetDataType(TKey key);
    }
}
