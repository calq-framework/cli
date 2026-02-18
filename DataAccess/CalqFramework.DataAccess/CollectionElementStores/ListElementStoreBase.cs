using System.Collections;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for list stores that provide key-value access to list elements.
/// </summary>
public abstract class ListElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    protected ListElementStoreBase(IList list) : base(list) {
    }

    protected IList List => (IList)ParentCollection;

    public override Type GetDataType(TKey key) {
        return List.GetType().GetGenericArguments()[0];
    }

    public override void Add(TValue value) {
        List.Add(value);
    }

    public override TValue AddNew() {
        object value = Activator.CreateInstance(List.GetType().GetGenericArguments()[0]) ??
            Activator.CreateInstance(Nullable.GetUnderlyingType(List.GetType().GetGenericArguments()[0])!)!;
        List.Add(value);
        return (TValue)value;
    }

    public override void Remove(TKey key) {
        if (key is string strKey && int.TryParse(strKey, out int index)) {
            List.RemoveAt(index);
        } else if (key is int intKey) {
            List.RemoveAt(intKey);
        } else {
            throw DataAccessErrors.InvalidListKey(key?.GetType().Name);
        }
    }
}
