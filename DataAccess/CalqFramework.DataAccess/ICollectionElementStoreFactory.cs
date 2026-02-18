using System.Collections;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating collection stores based on collection type.
/// </summary>
public interface ICollectionElementStoreFactory<TKey, TValue> {
    /// <summary>
    /// Creates an appropriate collection store for the given collection.
    /// </summary>
    ICollectionElementStore<TKey, TValue> CreateStore(ICollection collection);
}
