namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
///     Base class for array stores that provide key-value access to array elements.
/// </summary>
public abstract class ArrayElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {
    protected ArrayElementStoreBase(Array targetArray) => Array = targetArray;

    protected Array Array { get; }

    public override Type GetValueType(TKey key) => Array.GetType().GetElementType()!;
}
