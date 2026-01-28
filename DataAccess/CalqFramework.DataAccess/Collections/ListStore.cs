using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to list elements by index.
/// </summary>
public sealed class ListStore : CollectionStoreBase {

    public ListStore(IList list) : base(list) {
    }

    private IList List => (IList)ParentCollection;

    public override object? this[string key] {
        get => List[int.Parse(key)];
        set => List[int.Parse(key)] = value;
    }

    public override bool ContainsKey(string key) {
        if (!int.TryParse(key, out int index)) {
            return false;
        }
        return index >= 0 && index < List.Count;
    }

    public override Type GetDataType(string key) {
        return this[key]!.GetType();
    }

    public override object? GetValueOrInitialize(string key) {
        object? element = List[int.Parse(key)];
        if (element == null) {
            element = Activator.CreateInstance(List.GetType().GetGenericArguments()[0]!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(List.GetType().GetGenericArguments()[0])!)!;
            List[int.Parse(key)] = element;
        }
        return element;
    }

    public override void AddValue(object? value) {
        List.Add(value);
    }

    public override object AddValue() {
        object value = Activator.CreateInstance(List.GetType().GetGenericArguments()[0]) ??
            Activator.CreateInstance(Nullable.GetUnderlyingType(List.GetType().GetGenericArguments()[0])!)!;
        List.Add(value);
        return value;
    }

    public override void RemoveValue(string key) {
        List.RemoveAt(int.Parse(key));
    }
}
