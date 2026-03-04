using System.Collections;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
///     Base class for list stores that provide key-value access to list elements.
/// </summary>
public abstract class ListElementStoreBase<TKey, TValue>(IList targetList) : CollectionElementStoreBase<TKey, TValue> {
    protected IList List { get; } = targetList;

    public override Type GetValueType(TKey key) => List.GetType().GetGenericArguments()[0];

    public override void Add(TValue value) => List.Add(value);

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
