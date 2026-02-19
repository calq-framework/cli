using System.Collections;
using System.Linq;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for dictionary stores that provide key-value access to dictionary elements.
/// </summary>
public abstract class DictionaryElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    protected DictionaryElementStoreBase(IDictionary targetDictionary) {
        Dictionary = targetDictionary;
    }

    protected IDictionary Dictionary { get; }

    public override Type GetValueType(TKey key) {
        return Dictionary.GetType().GetGenericArguments()[1];
    }

    public override void Remove(TKey key) {
        Dictionary.Remove(key!);
    }
}
