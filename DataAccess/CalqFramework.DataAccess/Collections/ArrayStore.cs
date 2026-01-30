using System.Collections;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to array elements by index.
/// </summary>
public sealed class ArrayStore : ArrayStoreBase<string, object?> {

    public ArrayStore(Array array) : base(array) {
    }

    public override object? this[string key] {
        get {
            int index = int.Parse(key);
            return Array.GetValue(index);
        }
        set {
            int index = int.Parse(key);
            Array.SetValue(value, index);
        }
    }

    public override bool ContainsKey(string key) {
        try {
            int index = int.Parse(key);
            return index >= 0 && index < Array.Length;
        } catch {
            return false;
        }
    }

    public override object? GetValueOrInitialize(string key) {
        int index = int.Parse(key);
        object? element = Array.GetValue(index);
        if (element == null) {
            element = Activator.CreateInstance(Array.GetType().GetElementType()!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Array.GetType().GetElementType()!)!)!;
            Array.SetValue(element, index);
        }
        return element;
    }
}
