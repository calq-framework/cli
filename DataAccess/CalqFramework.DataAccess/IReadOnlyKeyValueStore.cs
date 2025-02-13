namespace CalqFramework.DataAccess {

    public interface IReadOnlyKeyValueStore<TKey, TValue> {
        TValue this[TKey key] { get; }

        bool ContainsKey(TKey key);

        Type GetDataType(TKey key);
    }
}