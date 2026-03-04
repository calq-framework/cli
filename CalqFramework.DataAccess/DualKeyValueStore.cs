namespace CalqFramework.DataAccess;

/// <summary>
///     Combines two key-value stores with primary and secondary lookup priority.
/// </summary>
public class DualKeyValueStore<TKey, TValue>(IKeyValueStore<TKey, TValue> primaryStore, IKeyValueStore<TKey, TValue> secondaryStore) : DualKeyValueStoreBase<TKey, TValue> {
    private readonly IKeyValueStore<TKey, TValue> _primaryStore = primaryStore;
    private readonly IKeyValueStore<TKey, TValue> _secondaryStore = secondaryStore;

    protected override IKeyValueStore<TKey, TValue> PrimaryStore => _primaryStore;

    protected override IKeyValueStore<TKey, TValue> SecondaryStore => _secondaryStore;
}
