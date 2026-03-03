using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for element stores that provide key-value access to enumerable elements.
/// </summary>
public abstract class CollectionElementStoreBase<TKey, TValue> : ICollectionElementStore<TKey, TValue> {

    public abstract TValue this[TKey key] { get; set; }

    public abstract bool ContainsKey(TKey key);

    public abstract Type GetValueType(TKey key);

    public abstract TValue GetValueOrInitialize(TKey key);

    public virtual void Add(TValue value) {
        throw DataAccessErrors.OperationNotSupported("Add");
    }

    public virtual IEnumerable Append(TValue value) {
        throw DataAccessErrors.OperationNotSupported("Append");
    }

    public virtual TValue AddNew() {
        throw DataAccessErrors.OperationNotSupported("AddNew");
    }

    public virtual void Remove(TKey key) {
        throw DataAccessErrors.OperationNotSupported("Remove");
    }
}
