using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to array elements by index.
/// </summary>
public sealed class ArrayStore : CollectionStoreBase {

    public ArrayStore(Array array) : base(array) {
    }

    private Array Array => (Array)ParentCollection;

    public override object? this[string key] {
        get => Array.GetValue(int.Parse(key));
        set => Array.SetValue(value, int.Parse(key));
    }

    public override bool ContainsKey(string key) {
        if (!int.TryParse(key, out int index)) {
            return false;
        }
        return index >= 0 && index < Array.Length;
    }

    public override Type GetDataType(string key) {
        return this[key]!.GetType();
    }

    public override object? GetValueOrInitialize(string key) {
        object? element = Array.GetValue(int.Parse(key));
        if (element == null) {
            element = Activator.CreateInstance(Array.GetType().GetElementType()!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Array.GetType().GetElementType()!)!)!;
            Array.SetValue(element, int.Parse(key));
        }
        return element;
    }
}
