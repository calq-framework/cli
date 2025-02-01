using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public abstract class DualKeyValueStoreBase<TKey, TValue> : IKeyValueStore<TKey, TValue> {
        public abstract IKeyValueStore<TKey, TValue> PrimaryAccessor { get; }
        public abstract IKeyValueStore<TKey, TValue> SecondaryAccessor { get; }

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
}