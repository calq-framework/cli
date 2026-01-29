using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to list elements by index.
/// </summary>
public sealed class ListStore : ListStoreBase<string, object?> {

    public ListStore(IList list) : base(list) {
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
            element = Activator.CreateInstance(List.GetType().GetGenericArguments()[0]!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(List.GetType().GetGenericArguments()[0])!)!;
            List[index] = element;
        }
        return element;
    }
}
