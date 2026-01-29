using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to list elements by index.
/// </summary>
public sealed class ListStore : ListStoreBase<string, object?> {

    private readonly IValueParser _valueParser;

    public ListStore(IList list, IValueParser valueParser) : base(list) {
        _valueParser = valueParser;
    }

    public override object? this[string key] {
        get {
            int index = _valueParser.ParseValue<int>(key);
            return List[index];
        }
        set {
            int index = _valueParser.ParseValue<int>(key);
            List[index] = value;
        }
    }

    public override bool ContainsKey(string key) {
        try {
            int index = _valueParser.ParseValue<int>(key);
            return index >= 0 && index < List.Count;
        } catch {
            return false;
        }
    }

    public override object? GetValueOrInitialize(string key) {
        int index = _valueParser.ParseValue<int>(key);
        object? element = List[index];
        if (element == null) {
            element = Activator.CreateInstance(List.GetType().GetGenericArguments()[0]!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(List.GetType().GetGenericArguments()[0])!)!;
            List[index] = element;
        }
        return element;
    }
}
