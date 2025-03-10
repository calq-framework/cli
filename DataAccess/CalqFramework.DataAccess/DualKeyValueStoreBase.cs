﻿namespace CalqFramework.DataAccess {

    public abstract class DualKeyValueStoreBase<TKey, TValue> : IKeyValueStore<TKey, TValue> {
        protected abstract IKeyValueStore<TKey, TValue> PrimaryStore { get; }
        protected abstract IKeyValueStore<TKey, TValue> SecondaryStore { get; }
        public TValue this[TKey key] {
            get {
                if (PrimaryStore.ContainsKey(key)) {
                    return PrimaryStore[key];
                } else {
                    return SecondaryStore[key];
                }
            }
            set {
                if (PrimaryStore.ContainsKey(key)) {
                    PrimaryStore[key] = value;
                } else {
                    SecondaryStore[key] = value;
                }
            }
        }

        public bool ContainsKey(TKey key) {
            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.ContainsKey(key);
            } else {
                return SecondaryStore.ContainsKey(key);
            }
        }

        public Type GetDataType(TKey key) {
            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.GetDataType(key);
            } else {
                return SecondaryStore.GetDataType(key);
            }
        }

        public TValue GetValueOrInitialize(TKey key) {
            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.GetValueOrInitialize(key);
            } else {
                return SecondaryStore.GetValueOrInitialize(key);
            }
        }
    }
}
