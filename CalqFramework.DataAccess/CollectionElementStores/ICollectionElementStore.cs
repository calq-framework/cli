using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Provides key-value access to collection elements with additional collection-specific operations.
/// </summary>
public interface ICollectionElementStore<TKey, TValue> : IKeyValueStore<TKey, TValue> {
    /// <summary>
    /// Adds a value to the collection (for mutable collections).
    /// Mutates the collection in place.
    /// </summary>
    void Add(TValue value);

    /// <summary>
    /// Appends a value to the collection and returns the result.
    /// For mutable collections (List, Array), delegates to Add() and returns the collection itself.
    /// For immutable collections (IEnumerable), creates a new enumerable with the value appended.
    /// </summary>
    IEnumerable Append(TValue value);

    /// <summary>
    /// Creates and adds a new instance to the collection (for list-like collections).
    /// </summary>
    TValue AddNew();

    /// <summary>
    /// Removes a value from the collection by key.
    /// </summary>
    void Remove(TKey key);
}
