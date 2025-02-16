using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {

    public interface IMediatedKeyValueStore<TKey, TAccessor, TInternalValue> {
        protected internal IEnumerable<TAccessor> Accessors { get; }

        protected internal TInternalValue this[TAccessor accessor] { get; set; }

        protected internal bool ContainsAccessor(TAccessor accessor);

        protected internal TAccessor GetAccessor(TKey key);

        protected internal Type GetDataType(TAccessor accessor);

        protected internal TInternalValue GetValueOrInitialize(TAccessor accessor);

        protected internal bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TAccessor result);
    }
}