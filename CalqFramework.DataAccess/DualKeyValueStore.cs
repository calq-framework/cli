namespace CalqFramework.DataAccess;

/// <summary>
///     Combines two key-value stores with primary and secondary lookup priority.
/// </summary>
public class DualKeyValueStore<TKey, TValue>(IKeyValueStore<TKey, TValue> primaryStore, IKeyValueStore<TKey, TValue> secondaryStore) : DualKeyValueStoreBase<TKey, TValue> {
    protected override IKeyValueStore<TKey, TValue> PrimaryStore { get; } = primaryStore;

    protected override IKeyValueStore<TKey, TValue> SecondaryStore { get; } = secondaryStore;
}
