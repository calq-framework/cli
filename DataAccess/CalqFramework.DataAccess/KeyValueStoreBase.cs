namespace CalqFramework.DataAccess {
    public abstract class KeyValueStoreBase<TKey, TValue, TAccessor> : KeyAccessorResolverBase<TKey, TAccessor>, IKeyValueStore<TKey, TValue, TAccessor> {
        public TValue this[TKey key] {
            get {
                var accessor = GetAccessor(key);
                return this[accessor];
            }
            set {
                var accessor = GetAccessor(key);
                this[accessor] = value;
            }
        }
        public abstract TValue this[TAccessor accessor] { get; set; }

        public abstract IEnumerable<TAccessor> Accessors { get; }

        public bool ContainsKey(TKey key) {
            return TryGetAccessor(key, out var _);
        }

        public Type GetDataType(TKey key) {
            var accessor = GetAccessor(key);
            return                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            GetDataType(accessor);
        }

        public TValue GetValueOrInitialize(TKey key) {
            var accessor = GetAccessor(key);
            return GetValueOrInitialize(accessor);
        }

        public abstract bool ContainsAccessor(TAccessor accessor);

        public abstract Type GetDataType(TAccessor accessor);

        public abstract TValue GetValueOrInitialize(TAccessor accessor);
    }
}