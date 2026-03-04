using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

internal class OptionStore(ICliKeyValueStore<string, string?, MemberInfo> store) : IOptionStore {
    private ICliKeyValueStore<string, string?, MemberInfo> Store { get; } = store;

    public string? this[string key] {
        get => Store[key];
        set => Store[key] = value;
    }

    public bool ContainsKey(string key) => Store.ContainsKey(key);

    public Type GetValueType(string key) => Store.GetValueType(key);

    public IEnumerable<Option> GetOptions() =>
        Store.GetAccessorKeysPairs().Select(pair => new Option {
            ValueType = GetValueType(pair.Keys[0]),
            IsMultiValue = Store.IsMultiValue(pair.Keys[0]),
            Keys = pair.Keys,
            MemberInfo = pair.Accessor,
            Value = this[pair.Keys[0]]
        });

    public string? GetValueOrInitialize(string key) => Store.GetValueOrInitialize(key);
}
