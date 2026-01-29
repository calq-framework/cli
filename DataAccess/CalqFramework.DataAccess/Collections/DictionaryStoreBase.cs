using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Base class for dictionary stores that provide key-value access to dictionary elements.
/// </summary>
public abstract class DictionaryStoreBase<TKey, TValue> : CollectionStoreBase<TKey, TValue> {

    protected DictionaryStoreBase(IDictionary dictionary) : base(dictionary) {
    }

    protected IDictionary Dictionary => (IDictionary)ParentCollection;

    public override Type GetDataType(TKey key) {
        return Dictionary.GetType().GetGenericArguments()[1];
    }

    public override void Remove(TKey key) {
        Dictionary.Remove(key);
    }
}
