namespace CalqFramework.DataAccess {

    public interface IKeyValueStore<TKey, TValue> : IReadOnlyKeyValueStore<TKey, TValue> {
        new TValue this[TKey key] { get; set; }

        TValue GetValueOrInitialize(TKey key);
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> : IKeyAccessorResolver<TKey, TAccessor>, IKeyValueStore<TKey, TValue>, IMediatedKeyValueStore<TAccessor, TInternalValue> {
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue, TAccessor, TValue> {
    }
}