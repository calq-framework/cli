using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    /// <summary>
    /// Combines two key-value stores with primary and secondary lookup priority.
    /// </summary>
    public class DualKeyValueStore<TKey, TValue> : DualKeyValueStoreBase<TKey, TValue> {
        private readonly IKeyValueStore<TKey, TValue> _primaryStore;
        private readonly IKeyValueStore<TKey, TValue> _secondaryStore;

        public DualKeyValueStore(IKeyValueStore<TKey, TValue> primaryStore, IKeyValueStore<TKey, TValue> secondaryStore) {
            _primaryStore = primaryStore;
            _secondaryStore = secondaryStore;
        }

        protected override IKeyValueStore<TKey, TValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<TKey, TValue> SecondaryStore => _secondaryStore;
    }
}
