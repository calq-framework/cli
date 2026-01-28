using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccess.Collections;

/// <summary>
/// Provides key-value access to dictionary elements.
/// </summary>
public sealed class DictionaryStore : CollectionStoreBase {

    private readonly IValueParser _valueParser;

    public DictionaryStore(IDictionary dictionary, IValueParser valueParser) : base(dictionary) {
        _valueParser = valueParser;
    }

    private IDictionary Dictionary => (IDictionary)ParentCollection;

    public override object? this[string key] {
        get {
            object parsedKey = _valueParser.ParseValue(key, Dictionary.GetType().GetGenericArguments()[0]);
            return Dictionary[parsedKey];
        }
        set {
            object parsedKey = _valueParser.ParseValue(key, Dictionary.GetType().GetGenericArguments()[0]);
            Dictionary[parsedKey] = value;
        }
    }

    public override bool ContainsKey(string key) {
        object parsedKey = _valueParser.ParseValue(key, Dictionary.GetType().GetGenericArguments()[0]);
        return Dictionary.Contains(parsedKey);
    }

    public override Type GetDataType(string key) {
        return this[key]!.GetType();
    }

    public override object? GetValueOrInitialize(string key) {
        object parsedKey = _valueParser.ParseValue(key, Dictionary.GetType().GetGenericArguments()[0]);
        object? element = Dictionary[parsedKey];
        if (element == null) {
            element = Activator.CreateInstance(Dictionary.GetType().GetGenericArguments()[1]!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Dictionary.GetType().GetGenericArguments()[1])!)!;
            Dictionary[parsedKey] = element;
        }
        return element;
    }

    public override void RemoveValue(string key) {
        Dictionary.Remove(key);
    }
}
