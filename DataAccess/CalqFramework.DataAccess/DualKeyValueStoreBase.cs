using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public abstract class DualKeyValueStoreBase<TKey, TValue> : IKeyValueStore<TKey, TValue> {
        public abstract IKeyValueStore<TKey, TValue> PrimaryStore { get; }
        public abstract IKeyValueStore<TKey, TValue> SecondaryStore { get; }

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

        private void AssertNoCollision(TKey key) {
            if (PrimaryStore.ContainsKey(key) && SecondaryStore.ContainsKey(key)) {
                throw new Exception("collision");
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

        public bool ContainsKey(TKey key) {
            AssertNoCollision(key);

            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.ContainsKey(key);
            } else {
                return SecondaryStore.ContainsKey(key);
            }
        }
    }

    public abstract class DualKeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> : IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> {
        public abstract IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> PrimaryStore { get; }
        public abstract IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> SecondaryStore { get; }

        public IEnumerable<TAccessor> Accessors => PrimaryStore.Accessors.Concat(SecondaryStore.Accessors);

        public virtual TInternalValue this[TAccessor accessor] {
            get {
                if (PrimaryStore.ContainsAccessor(accessor)) {
                    return PrimaryStore[accessor];
                } else {
                    return SecondaryStore[accessor];
                }
            }
            set {
                if (PrimaryStore.ContainsAccessor(accessor)) {
                    PrimaryStore[accessor] = value;
                } else {
                    SecondaryStore[accessor] = value;
                }
            }
        }

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

        private void AssertNoCollision(TKey key) {
            if (PrimaryStore.ContainsKey(key) && SecondaryStore.ContainsKey(key)) {
                throw new Exception("collision");
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

        public bool ContainsKey(TKey key) {
            AssertNoCollision(key);

            if (PrimaryStore.ContainsKey(key)) {
                return PrimaryStore.ContainsKey(key);
            } else {
                return SecondaryStore.ContainsKey(key);
            }
        }

        public Type GetDataType(TAccessor accessor) {
            if (PrimaryStore.ContainsAccessor(accessor)) {
                return PrimaryStore.GetDataType(accessor);
            } else {
                return SecondaryStore.GetDataType(accessor);
            }
        }

        public TInternalValue GetValueOrInitialize(TAccessor accessor) {
            if (PrimaryStore.ContainsAccessor(accessor)) {
                return PrimaryStore.GetValueOrInitialize(accessor);
            } else {
                return SecondaryStore.GetValueOrInitialize(accessor);
            }
        }

        public bool ContainsAccessor(TAccessor accessor) {
            if (PrimaryStore.ContainsAccessor(accessor)) {
                return PrimaryStore.ContainsAccessor(accessor);
            } else {
                return SecondaryStore.ContainsAccessor(accessor);
            }
        }

        public bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TAccessor result) {
            PrimaryStore.TryGetAccessor(key, out result);
            if (result == null) {
                SecondaryStore.TryGetAccessor(key, out result);
            }
            return result != null;
        }

        public TAccessor GetAccessor(TKey key) {
            PrimaryStore.TryGetAccessor(key, out var result);
            if (result == null) {
                return SecondaryStore.GetAccessor(key);
            }
            return result;
        }
    }
}