namespace CalqFramework.DataAccess {

    public interface IKeyValueStore<TKey, TValue> {
        public TValue this[TKey key] { get; set; }

        bool ContainsKey(TKey key);

        Type GetDataType(TKey key);

        TValue GetValueOrInitialize(TKey key);
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue, TAccessor, TValue> {
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> : IKeyAccessorResolver<TKey, TAccessor>, IKeyValueStore<TKey, TValue>, IMediatedKeyValueStore<TAccessor, TInternalValue> {
    }
}