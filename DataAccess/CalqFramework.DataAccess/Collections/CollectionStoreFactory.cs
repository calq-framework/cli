using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Factory for creating collection stores based on collection type.
/// </summary>
public sealed class CollectionStoreFactory : CollectionStoreFactoryBase {

    protected override CollectionStoreBase CreateArrayStore(Array array) {
        return new ArrayStore(array);
    }

    protected override CollectionStoreBase CreateListStore(IList list) {
        return new ListStore(list);
    }

    protected override CollectionStoreBase CreateDictionaryStore(IDictionary dictionary) {
        return new DictionaryStore(dictionary, ValueParser);
    }
}
