using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public class DualKeyValueStore<TKey, TValue> : IKeyValueStore<TKey, TValue> {
        public IKeyValueStore<TKey, TValue> PrimaryAccessor { get; }
        public IKeyValueStore<TKey, TValue> SecondaryAccessor { get; }

        public TValue this[TKey key] {
            get {
                AssertNoCollision(key);

                if (PrimaryAccessor.ContainsKey(key)) {
                    return PrimaryAccessor[key];
                } else {
                    return SecondaryAccessor[key];
                }
            }
            set {
                AssertNoCollision(key);

                if (PrimaryAccessor.ContainsKey(key)) {
                    PrimaryAccessor[key] = value;
                } else {
                    SecondaryAccessor[key] = value;
                }
            }
        }

        public DualKeyValueStore(IKeyValueStore<TKey, TValue> primaryAccessor, IKeyValueStore<TKey, TValue> secondaryAccessor) {
            PrimaryAccessor = primaryAccessor;
            SecondaryAccessor = secondaryAccessor;
        }

        private void AssertNoCollision(TKey key) {
            if (PrimaryAccessor.ContainsKey(key) && SecondaryAccessor.ContainsKey(key)) {
                throw new Exception("collision");
            }
        }

        public Type GetDataType(TKey key) {
            AssertNoCollision(key);

            if (PrimaryAccessor.ContainsKey(key)) {
                return PrimaryAccessor.GetDataType(key);
            } else {
                return SecondaryAccessor.GetDataType(key);
            }
        }

        public TValue GetValueOrInitialize(TKey key) {
            AssertNoCollision(key);

            if (PrimaryAccessor.ContainsKey(key)) {
                return PrimaryAccessor.GetValueOrInitialize(key);
            } else {
                return SecondaryAccessor.GetValueOrInitialize(key);
            }
        }

        public bool ContainsKey(TKey key) {
            AssertNoCollision(key);

            if (PrimaryAccessor.ContainsKey(key)) {
                return PrimaryAccessor.ContainsKey(key);
            } else {
                return SecondaryAccessor.ContainsKey(key);
            }
        }
    }

    public class DualKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue, TAccessor> {
        public IKeyValueStore<TKey, TValue, TAccessor> PrimaryStore { get; }
        public IKeyValueStore<TKey, TValue, TAccessor> SecondaryStore { get; }

        public IEnumerable<TAccessor> Accessors => PrimaryStore.Accessors.Concat(SecondaryStore.Accessors);

        public virtual TValue this[TAccessor accessor] {
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

        public DualKeyValueStore(IKeyValueStore<TKey, TValue, TAccessor> primaryStore, IKeyValueStore<TKey, TValue, TAccessor> secondaryStore) {
            PrimaryStore = primaryStore;
            SecondaryStore = secondaryStore;
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

        public TValue GetValueOrInitialize(TAccessor accessor) {
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
            return PrimaryStore.GetAccessor(key) ?? SecondaryStore.GetAccessor(key); ;
        }

        public string AccessorToString(TAccessor accessor) {
            if (PrimaryStore.ContainsAccessor(accessor)) {
                return PrimaryStore.AccessorToString(accessor);
            } else {
                return SecondaryStore.AccessorToString(accessor);
            }
        }
    }
}