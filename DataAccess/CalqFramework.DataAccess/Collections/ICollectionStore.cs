namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to collection elements with additional collection-specific operations.
/// </summary>
public interface ICollectionStore<TKey, TValue> : IKeyValueStore<TKey, TValue> {
    /// <summary>
    /// Adds a value to the collection (for list-like collections).
    /// </summary>
    void Add(TValue value);

    /// <summary>
    /// Creates and adds a new instance to the collection (for list-like collections).
    /// </summary>
    TValue AddNew();

    /// <summary>
    /// Removes a value from the collection by key.
    /// </summary>
    void Remove(TKey key);
}
