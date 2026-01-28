namespace CalqFramework.DataAccess {

    public abstract class DistinctDualKeyValueStoreBase<TKey, TValue> : IKeyValueStore<TKey, TValue> {
        public bool EnableShadowing { get; init; }
        protected abstract IKeyValueStore<TKey, TValue> PrimaryStore { get; }
        protected abstract IKeyValueStore<TKey, TValue> SecondaryStore { get; }
        public TValue this[TKey key] {
            get {
                AssertNoCollision(key);

                if (PrimaryStore.ContainsKey(key)) {
                    return PrimaryStore[key];
                } else {
                    return SecondaryStore[key];
                }
            }
            set {
                AssertNoCollision(key);

                if (PrimaryStore.ContainsKey(key)) {
                    PrimaryStore[key] = value;
                } else {
                    SecondaryStore[key] = value;
                }
            }
        }

        public bool ContainsKey(TKey key) {
            AssertNoCollision(key);

            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.ContainsKey(key);
            } else {
                return SecondaryStore.ContainsKey(key);
            }
        }

        public Type GetDataType(TKey key) {
            AssertNoCollision(key);

            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.GetDataType(key);
            } else {
                return SecondaryStore.GetDataType(key);
            }
        }

        public TValue GetValueOrInitialize(TKey key) {
            AssertNoCollision(key);

            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.GetValueOrInitialize(key);
            } else {
                return SecondaryStore.GetValueOrInitialize(key);
            }
        }

        private void AssertNoCollision(TKey key) {
            if (!EnableShadowing) {
                if (PrimaryStore.ContainsKey(key) && SecondaryStore.ContainsKey(key)) {
                    throw new Exception($"Ambigious key {key}. Enable shadowing to ignore this error.");
                }
            }
        }
    }
}
