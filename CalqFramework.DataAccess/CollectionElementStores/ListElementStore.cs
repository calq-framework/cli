using System.Collections;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Provides key-value access to list elements by index.
/// </summary>
public sealed class ListElementStore : ListElementStoreBase<string, object?> {

    public ListElementStore(IList list) : base(list) {
    }

    public override object? this[string key] {
        get {
            int index = int.Parse(key);
            return List[index];
        }
        set {
            int index = int.Parse(key);
            List[index] = value;
        }
    }

    public override bool ContainsKey(string key) {
        if (!int.TryParse(key, out int index)) {
            return false;
        }
        return index >= 0 && index < List.Count;
    }

    public override object? GetValueOrInitialize(string key) {
        int index = int.Parse(key);
        object? element = List[index];
        if (element == null) {
            element = List.GetType().GetGenericArguments()[0].CreateInstance();
            List[index] = element;
        }
        return element;
    }

    public override void Remove(string key) {
        if (!int.TryParse(key, out int index)) {
            throw DataAccessErrors.InvalidListKey(key?.GetType().Name);
        }
        List.RemoveAt(index);
    }
}
