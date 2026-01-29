using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Base class for collection store factories.
/// </summary>
public abstract class CollectionStoreFactoryBase<TKey, TValue> : ICollectionStoreFactory<TKey, TValue> {

    public IValueParser IndexParser { get; init; } = new ValueParser();
    public IValueParser KeyParser { get; init; } = new ValueParser();

    public virtual ICollectionStore<TKey, TValue> CreateStore(ICollection collection) {
        return collection switch {
            Array array => CreateArrayStore(array),
            IList list => CreateListStore(list),
            IDictionary dictionary => CreateDictionaryStore(dictionary),
            _ => throw new ArgumentException($"Unsupported collection type: {collection.GetType().Name}", nameof(collection))
        };
    }

    protected abstract ICollectionStore<TKey, TValue> CreateArrayStore(Array array);
    protected abstract ICollectionStore<TKey, TValue> CreateListStore(IList list);
    protected abstract ICollectionStore<TKey, TValue> CreateDictionaryStore(IDictionary dictionary);
}
