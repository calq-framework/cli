namespace CalqFramework.DataAccess {

    public interface IKeyValueStore<TKey, TValue> : IReadOnlyKeyValueStore<TKey, TValue> {
        new TValue this[TKey key] { get; set; }

        TValue GetValueOrInitialize(TKey key);
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> : IKeyValueStore<TKey, TValue>, IMediatedKeyValueStore<TKey, TAccessor, TInternalValue> {
    }

    public interface IKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue, TAccessor, TValue> {
    }
}
