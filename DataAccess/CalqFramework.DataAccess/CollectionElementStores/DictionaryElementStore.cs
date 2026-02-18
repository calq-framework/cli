using System.Collections;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Provides key-value access to dictionary elements.
/// </summary>
public sealed class DictionaryElementStore : DictionaryElementStoreBase<string, object?> {

    public DictionaryElementStore(IDictionary dictionary) : base(dictionary) {
    }

    public override object? this[string key] {
        get {
            object parsedKey = Dictionary.GetType().GetGenericArguments()[0].Parse(key);
            return Dictionary[parsedKey];
        }
        set {
            object parsedKey = Dictionary.GetType().GetGenericArguments()[0].Parse(key);
            Dictionary[parsedKey] = value;
        }
    }

    public override bool ContainsKey(string key) {
        try {
            object parsedKey = Dictionary.GetType().GetGenericArguments()[0].Parse(key);
            return Dictionary.Contains(parsedKey);
        } catch {
            return false;
        }
    }

    public override object? GetValueOrInitialize(string key) {
        object parsedKey = Dictionary.GetType().GetGenericArguments()[0].Parse(key);
        object? element = Dictionary[parsedKey];
        if (element == null) {
            element = Activator.CreateInstance(Dictionary.GetType().GetGenericArguments()[1]!) ??
                Activator.CreateInstance(Nullable.GetUnderlyingType(Dictionary.GetType().GetGenericArguments()[1])!)!;
            Dictionary[parsedKey] = element;
        }
        return element;
    }
}
