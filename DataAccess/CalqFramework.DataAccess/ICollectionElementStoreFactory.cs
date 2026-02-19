using System.Collections;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating element stores based on enumerable type.
/// </summary>
public interface ICollectionElementStoreFactory<TKey, TValue> {
    /// <summary>
    /// Creates an appropriate element store for the given enumerable.
    /// </summary>
    ICollectionElementStore<TKey, TValue> CreateStore(IEnumerable enumerable);
}
