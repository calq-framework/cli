using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Base class for collection stores that provide key-value access to collection elements.
/// </summary>
public abstract class CollectionStoreBase<TKey, TValue> : ICollectionStore<TKey, TValue> {

    protected CollectionStoreBase(ICollection collection) {
        ParentCollection = collection;
    }

    protected ICollection ParentCollection { get; }

    public abstract TValue this[TKey key] { get; set; }

    public abstract bool ContainsKey(TKey key);

    public abstract Type GetDataType(TKey key);

    public abstract TValue GetValueOrInitialize(TKey key);

    /// <summary>
    /// Adds a value to the collection (for list-like collections).
    /// </summary>
    public virtual void Add(TValue value) {
        throw new NotSupportedException($"Add is not supported for {ParentCollection.GetType().Name}");
    }

    /// <summary>
    /// Creates and adds a new instance to the collection (for list-like collections).
    /// </summary>
    public virtual TValue AddNew() {
        throw new NotSupportedException($"AddNew is not supported for {ParentCollection.GetType().Name}");
    }

    /// <summary>
    /// Removes a value from the collection by key.
    /// </summary>
    public virtual void Remove(TKey key) {
        throw new NotSupportedException($"Remove is not supported for {ParentCollection.GetType().Name}");
    }
}
