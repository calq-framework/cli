namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
///     Base class for array stores that provide key-value access to array elements.
/// </summary>
public abstract class ArrayElementStoreBase<TKey, TValue>(Array targetArray) : CollectionElementStoreBase<TKey, TValue> {
    protected Array Array { get; } = targetArray;

    public override Type GetValueType(TKey key) => Array.GetType()
        .GetElementType()!;
}
