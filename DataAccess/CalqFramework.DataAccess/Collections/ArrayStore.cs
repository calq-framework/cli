using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to array elements by index.
/// </summary>
public sealed class ArrayStore : ArrayStoreBase<string, object?> {

    private readonly IStringParser _stringParser;

    public ArrayStore(Array array, IStringParser stringParser) : base(array) {
        _stringParser = stringParser;
    }

    public override object? this[string key] {
        get {
            int index = _stringParser.ParseValue<int>(key);
            return Array.GetValue(index);
        }
        set {
            int index = _stringParser.ParseValue<int>(key);
            Array.SetValue(value, index);
        }
    }

    public override bool ContainsKey(string key) {
        try {
            int index = _stringParser.ParseValue<int>(key);
            return index >= 0 && index < Array.Length;
        } catch {
            return false;
        }
    }

    public override object? GetValueOrInitialize(string key) {
        int index = _stringParser.ParseValue<int>(key);
        object? element = Array.GetValue(index);
        if (element == null) {
            element = Activator.CreateInstance(Array.GetType().GetElementType()!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Array.GetType().GetElementType()!)!)!;
            Array.SetValue(element, index);
        }
        return element;
    }
}
