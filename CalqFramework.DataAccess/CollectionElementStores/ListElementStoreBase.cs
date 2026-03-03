using System.Collections;
using System.Linq;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for list stores that provide key-value access to list elements.
/// </summary>
public abstract class ListElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    protected ListElementStoreBase(IList targetList) {
        List = targetList;
    }

    protected IList List { get; }

    public override Type GetValueType(TKey key) {
        return List.GetType().GetGenericArguments()[0];
    }

    public override void Add(TValue value) {
        List.Add(value);
    }

    public override IEnumerable Append(TValue value) {
        Add(value);
        return List;
    }

    public override TValue AddNew() {
        object value = List.GetType().GetGenericArguments()[0].CreateInstance();
        List.Add(value);
        return (TValue)value;
    }
}
