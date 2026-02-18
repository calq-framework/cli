using System.Collections;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating collection stores with string keys and nullable object values.
/// </summary>
public sealed class CollectionElementStoreFactory : CollectionElementStoreFactoryBase<string, object?> {

    protected override ICollectionElementStore<string, object?> CreateArrayStore(Array array) {
        return new ArrayElementStore(array);
    }

    protected override ICollectionElementStore<string, object?> CreateListStore(IList list) {
        return new ListElementStore(list);
    }

    protected override ICollectionElementStore<string, object?> CreateDictionaryStore(IDictionary dictionary) {
        return new DictionaryElementStore(dictionary);
    }
}
