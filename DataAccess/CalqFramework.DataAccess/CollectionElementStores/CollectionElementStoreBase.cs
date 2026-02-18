using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for collection stores that provide key-value access to collection elements.
/// </summary>
public abstract class CollectionElementStoreBase<TKey, TValue> : ICollectionElementStore<TKey, TValue> {

    protected CollectionElementStoreBase(ICollection targetCollection) {
        TargetCollection = targetCollection;
    }

    protected ICollection TargetCollection { get; }

    public abstract TValue this[TKey key] { get; set; }

    public abstract bool ContainsKey(TKey key);

    public abstract Type GetDataType(TKey key);

    public abstract TValue GetValueOrInitialize(TKey key);

    /// <summary>
    /// Adds a value to the collection (for list-like collections).
    /// </summary>
    public virtual void Add(TValue value) {
        throw DataAccessErrors.OperationNotSupported("Add", TargetCollection.GetType().Name);
    }

    /// <summary>
    /// Creates and adds a new instance to the collection (for list-like collections).
    /// </summary>
    public virtual TValue AddNew() {
        throw DataAccessErrors.OperationNotSupported("AddNew", TargetCollection.GetType().Name);
    }

    /// <summary>
    /// Removes a value from the collection by key.
    /// </summary>
    public virtual void Remove(TKey key) {
        throw DataAccessErrors.OperationNotSupported("Remove", TargetCollection.GetType().Name);
    }
}
