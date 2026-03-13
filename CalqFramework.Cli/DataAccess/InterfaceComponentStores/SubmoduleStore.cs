using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

internal sealed class SubmoduleStore(ICliKeyValueStore<string, object?, MemberInfo> store) : ISubmoduleStore {
    private ICliKeyValueStore<string, object?, MemberInfo> Store { get; } = store;

    public object? this[string key] {
        get => Store[key];
        set => Store[key] = value;
    }

    public bool ContainsKey(string key) => Store.ContainsKey(key);

    public Type GetValueType(string key) => Store.GetValueType(key);

    public IEnumerable<Submodule> GetSubmodules() =>
        Store.GetAccessorKeysPairs()
            .Select(pair => new Submodule {
                Keys = pair.Keys,
                MemberInfo = pair.Accessor
            });

    public object? GetValueOrInitialize(string key) => Store.GetValueOrInitialize(key);
}
