using System.Collections;
using System.Linq;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.DataAccess;

/// <summary>
/// Base class for element store factories.
/// </summary>
public abstract class CollectionElementStoreFactoryBase<TKey, TValue> : ICollectionElementStoreFactory<TKey, TValue> {

    public virtual ICollectionElementStore<TKey, TValue> CreateStore(IEnumerable enumerable) {
        return enumerable switch {
            Array array => CreateArrayElementStore(array),
            IDictionary dictionary => CreateDictionaryElementStore(dictionary),
            IList list => CreateListElementStore(list),
            _ when IsGenericCollection(enumerable) => CreateGenericCollectionElementStore(enumerable),
            _ => CreateEnumerableElementStore(enumerable)
        };
    }

    private static bool IsGenericCollection(IEnumerable enumerable) {
        var type = enumerable.GetType();
        return type.GetInterfaces().Any(i => 
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    protected virtual ICollectionElementStore<TKey, TValue> CreateGenericCollectionElementStore(IEnumerable collection) {
        var type = collection.GetType();
        if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISet<>))) {
            return CreateSetGenericCollectionElementStore(collection);
        }
        return CreateEnumerableElementStore(collection);
    }

    protected abstract ICollectionElementStore<TKey, TValue> CreateArrayElementStore(Array array);
    protected abstract ICollectionElementStore<TKey, TValue> CreateListElementStore(IList list);
    protected abstract ICollectionElementStore<TKey, TValue> CreateDictionaryElementStore(IDictionary dictionary);
    protected abstract ICollectionElementStore<TKey, TValue> CreateSetGenericCollectionElementStore(IEnumerable collection);
    protected abstract ICollectionElementStore<TKey, TValue> CreateEnumerableElementStore(IEnumerable enumerable);
}
