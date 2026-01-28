using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Base class for collection store factories.
/// </summary>
public abstract class CollectionStoreFactoryBase : ICollectionStoreFactory {

    public IValueParser ValueParser { get; init; } = new ValueParser();

    public virtual CollectionStoreBase CreateStore(ICollection collection) {
        return collection switch {
            Array array => CreateArrayStore(array),
            IList list => CreateListStore(list),
            IDictionary dictionary => CreateDictionaryStore(dictionary),
            _ => throw new ArgumentException($"Unsupported collection type: {collection.GetType().Name}", nameof(collection))
        };
    }

    protected abstract CollectionStoreBase CreateArrayStore(Array array);
    protected abstract CollectionStoreBase CreateListStore(IList list);
    protected abstract CollectionStoreBase CreateDictionaryStore(IDictionary dictionary);
}
