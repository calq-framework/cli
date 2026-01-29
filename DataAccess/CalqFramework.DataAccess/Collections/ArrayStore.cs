using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to array elements by index.
/// </summary>
public sealed class ArrayStore : ArrayStoreBase<string, object?> {

    private readonly IValueParser _valueParser;

    public ArrayStore(Array array, IValueParser valueParser) : base(array) {
        _valueParser = valueParser;
    }

    public override object? this[string key] {
        get {
            int index = _valueParser.ParseValue<int>(key);
            return Array.GetValue(index);
        }
        set {
            int index = _valueParser.ParseValue<int>(key);
            Array.SetValue(value, index);
        }
    }

    public override bool ContainsKey(string key) {
        try {
            int index = _valueParser.ParseValue<int>(key);
            return index >= 0 && index < Array.Length;
        } catch {
            return false;
        }
    }

    public override object? GetValueOrInitialize(string key) {
        int index = _valueParser.ParseValue<int>(key);
        object? element = Array.GetValue(index);
        if (element == null) {
            element = Activator.CreateInstance(Array.GetType().GetElementType()!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Array.GetType().GetElementType()!)!)!;
            Array.SetValue(element, index);
        }
        return element;
    }
}
