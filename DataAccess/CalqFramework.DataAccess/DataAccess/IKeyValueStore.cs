namespace CalqFramework.Serialization.DataAccess {

    public interface IKeyValueStore<TKey, TValue> {
        public TValue this[TKey key] { get; set; }

        bool ContainsKey(TKey key);

        Type GetDataType(TKey key);

        TValue GetValueOrInitialize(TKey key);
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor> : IKeyAccessorResolver<TKey, TAccessor>, IKeyValueStore<TKey, TValue>, IMediatedKeyValueStore<TAccessor, TValue> {
        internal protected bool ContainsAccessor(TAccessor accessor);
    }
}