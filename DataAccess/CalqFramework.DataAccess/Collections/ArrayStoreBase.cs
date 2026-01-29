using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Base class for array stores that provide key-value access to array elements.
/// </summary>
public abstract class ArrayStoreBase<TKey, TValue> : CollectionStoreBase<TKey, TValue> {

    protected ArrayStoreBase(Array array) : base(array) {
    }

    protected Array Array => (Array)ParentCollection;

    public override Type GetDataType(TKey key) {
        return Array.GetType().GetElementType()!;
    }
}

