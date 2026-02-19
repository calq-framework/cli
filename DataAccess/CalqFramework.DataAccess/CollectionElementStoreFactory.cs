using System.Collections;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating element stores with string keys and nullable object values.
/// </summary>
public sealed class CollectionElementStoreFactory : CollectionElementStoreFactoryBase<string, object?> {

    protected override ICollectionElementStore<string, object?> CreateArrayElementStore(Array array) {
        return new ArrayElementStore(array);
    }

    protected override ICollectionElementStore<string, object?> CreateListElementStore(IList list) {
        return new ListElementStore(list);
    }

    protected override ICollectionElementStore<string, object?> CreateDictionaryElementStore(IDictionary dictionary) {
        return new DictionaryElementStore(dictionary);
    }

    protected override ICollectionElementStore<string, object?> CreateSetGenericCollectionElementStore(IEnumerable collection) {
        return new SetElementStore(collection);
    }

    protected override ICollectionElementStore<string, object?> CreateEnumerableElementStore(IEnumerable enumerable) {
        return new EnumerableElementStore(enumerable);
    }
}
