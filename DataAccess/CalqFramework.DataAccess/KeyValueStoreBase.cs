namespace CalqFramework.DataAccess {
    public abstract class KeyValueStoreBase<TKey, TValue, TAccessor> : KeyValueStoreBase<TKey, TValue, TAccessor, TValue>, IKeyValueStore<TKey, TValue, TAccessor> {
        protected override TValue ConvertFromInternalValue(TAccessor accessor, TValue value) {
            return value;
        }

        protected override TValue ConvertToInternalValue(TAccessor accessor, TValue value) {
            return value;
        }
    }

    public abstract class KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> : KeyAccessorResolverBase<TKey, TAccessor>, IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> {
        public TValue this[TKey key] {
            get {
                var accessor = GetAccessor(key);
                return ConvertFromInternalValue(accessor, this[accessor]);
            }
            set {
                var accessor = GetAccessor(key);
                this[accessor] = ConvertToInternalValue(accessor, value);
            }
        }
        public abstract TInternalValue this[TAccessor accessor] { get; set; }

        public abstract IEnumerable<TAccessor> Accessors { get; }

        public bool ContainsKey(TKey key) {
            return TryGetAccessor(key, out var _);
        }

        public Type GetDataType(TKey key) {
            var accessor = GetAccessor(key);
            return GetDataType(accessor);
        }

        public TValue GetValueOrInitialize(TKey key) {
            var accessor = GetAccessor(key);
            return ConvertFromInternalValue(accessor, GetValueOrInitialize(accessor));
        }

        public abstract bool ContainsAccessor(TAccessor accessor);

        public abstract Type GetDataType(TAccessor accessor);

        public abstract TInternalValue GetValueOrInitialize(TAccessor accessor);

        protected abstract TValue ConvertFromInternalValue(TAccessor accessor, TInternalValue value);

        protected abstract TInternalValue ConvertToInternalValue(TAccessor accessor, TValue value);
    }
}