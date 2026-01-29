using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Factory for creating collection stores with string keys and nullable object values.
/// </summary>
public sealed class CollectionStoreFactory : CollectionStoreFactoryBase<string, object?> {

    protected override ICollectionStore<string, object?> CreateArrayStore(Array array) {
        return new ArrayStore(array);
    }

    protected override ICollectionStore<string, object?> CreateListStore(IList list) {
        return new ListStore(list);
    }

    protected override ICollectionStore<string, object?> CreateDictionaryStore(IDictionary dictionary) {
        return new DictionaryStore(dictionary, ValueParser);
    }
}
