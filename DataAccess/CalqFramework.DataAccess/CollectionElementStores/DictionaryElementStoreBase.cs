using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for dictionary stores that provide key-value access to dictionary elements.
/// </summary>
public abstract class DictionaryElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    protected DictionaryElementStoreBase(IDictionary targetDictionary) : base(targetDictionary) {
    }

    protected IDictionary Dictionary => (IDictionary)TargetCollection;

    public override Type GetDataType(TKey key) {
        return Dictionary.GetType().GetGenericArguments()[1];
    }

    public override void Remove(TKey key) {
        Dictionary.Remove(key!);
    }
}
