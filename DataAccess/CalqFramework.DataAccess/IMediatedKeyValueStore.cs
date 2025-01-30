namespace CalqFramework.DataAccess {
    public interface IMediatedKeyValueStore<TAccessor, TValue> {
        protected internal TValue this[TAccessor accessor] { get; set; }

        protected internal IEnumerable<TAccessor> Accessors { get; }

        protected internal Type GetDataType(TAccessor accessor);

        protected internal TValue GetValueOrInitialize(TAccessor accessor);
    }
}