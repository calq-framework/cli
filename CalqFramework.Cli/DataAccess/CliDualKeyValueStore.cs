using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess;

internal sealed class CliDualKeyValueStore<TValue>(ICliKeyValueStore<string, TValue, MemberInfo> primaryStore, ICliKeyValueStore<string, TValue, MemberInfo> secondaryStore)
    : DualKeyValueStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {
    private readonly ICliKeyValueStore<string, TValue, MemberInfo> _primaryStore = primaryStore;
    private readonly ICliKeyValueStore<string, TValue, MemberInfo> _secondaryStore = secondaryStore;

    protected override IKeyValueStore<string, TValue> PrimaryStore => _primaryStore;

    protected override IKeyValueStore<string, TValue> SecondaryStore => _secondaryStore;

    public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() =>
        _primaryStore.GetAccessorKeysPairs()
            .Concat(_secondaryStore.GetAccessorKeysPairs());

    public bool IsMultiValue(string key) {
        if (_primaryStore.ContainsKey(key)) {
            return _primaryStore.IsMultiValue(key);
        }

        return _secondaryStore.IsMultiValue(key);
    }
}
