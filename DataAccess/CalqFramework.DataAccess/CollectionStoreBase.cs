using System.Collections;

namespace CalqFramework.DataAccess;

/// <summary>
/// Base class for collection stores that provide key-value access to collection elements.
/// </summary>
public abstract class CollectionStoreBase : IKeyValueStore<string, object?> {

    protected CollectionStoreBase(ICollection collection) {
        ParentCollection = collection;
    }

    protected ICollection ParentCollection { get; }

    public abstract object? this[string key] { get; set; }

    public abstract bool ContainsKey(string key);

    public abstract Type GetDataType(string key);

    public abstract object? GetValueOrInitialize(string key);

    /// <summary>
    /// Adds a value to the collection (for list-like collections).
    /// </summary>
    public virtual void AddValue(object? value) {
        throw new NotSupportedException($"AddValue is not supported for {ParentCollection.GetType().Name}");
    }

    /// <summary>
    /// Creates and adds a new instance to the collection (for list-like collections).
    /// </summary>
    public virtual object AddValue() {
        throw new NotSupportedException($"AddValue is not supported for {ParentCollection.GetType().Name}");
    }

    /// <summary>
    /// Removes a value from the collection by key.
    /// </summary>
    public virtual void RemoveValue(string key) {
        throw new NotSupportedException($"RemoveValue is not supported for {ParentCollection.GetType().Name}");
    }
}
