using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public class DualKeyValueStore<TKey, TValue> : DualKeyValueStoreBase<TKey, TValue> {
        private IKeyValueStore<TKey, TValue> _primaryStore;
        private IKeyValueStore<TKey, TValue> _secondaryStore;

        public DualKeyValueStore(IKeyValueStore<TKey, TValue> primaryStore, IKeyValueStore<TKey, TValue> secondaryStore) {
            _primaryStore = primaryStore;
            _secondaryStore = secondaryStore;
        }

        protected override IKeyValueStore<TKey, TValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<TKey, TValue> SecondaryStore => _secondaryStore;
    }

    public class DualKeyValueStore<TKey, TValue, TAccessor, TInternalValue> : DualKeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> {
        private IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> _primaryStore;
        private IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> _secondaryStore;

        public DualKeyValueStore(IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> primaryStore, IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> secondaryStore) {
            _primaryStore = primaryStore;
            _secondaryStore = secondaryStore;
        }

        protected override IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> SecondaryStore => _secondaryStore;
    }

    public class DualKeyValueStore<TKey, TValue, TAccessor> : DualKeyValueStore<TKey, TValue, TAccessor, TValue>, IKeyValueStore<TKey, TValue, TAccessor> {
        public DualKeyValueStore(IKeyValueStore<TKey, TValue, TAccessor, TValue> primaryStore, IKeyValueStore<TKey, TValue, TAccessor, TValue> secondaryStore) : base(primaryStore, secondaryStore) {
        }
    }
}