using System.Collections;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Base class for collection store factories.
/// </summary>
public abstract class CollectionElementStoreFactoryBase<TKey, TValue> : ICollectionElementStoreFactory<TKey, TValue> {

    public virtual ICollectionElementStore<TKey, TValue> CreateStore(ICollection collection) {
        return collection switch {
            Array array => CreateArrayStore(array),
            IList list => CreateListStore(list),
            IDictionary dictionary => CreateDictionaryStore(dictionary),
            _ => throw DataAccessErrors.UnsupportedCollectionType(collection.GetType().Name)
        };
    }

    protected abstract ICollectionElementStore<TKey, TValue> CreateArrayStore(Array array);
    protected abstract ICollectionElementStore<TKey, TValue> CreateListStore(IList list);
    protected abstract ICollectionElementStore<TKey, TValue> CreateDictionaryStore(IDictionary dictionary);
}
