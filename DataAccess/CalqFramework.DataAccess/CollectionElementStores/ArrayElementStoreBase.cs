using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for array stores that provide key-value access to array elements.
/// </summary>
public abstract class ArrayElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    protected ArrayElementStoreBase(Array array) : base(array) {
    }

    protected Array Array => (Array)ParentCollection;

    public override Type GetDataType(TKey key) {
        return Array.GetType().GetElementType()!;
    }
}

