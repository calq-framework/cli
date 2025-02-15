using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public abstract class KeyValueStoreBase<TKey, TValue, TAccessor> : KeyValueStoreBase<TKey, TValue, TAccessor, TValue>, IKeyValueStore<TKey, TValue, TAccessor> {
        protected override TValue ConvertFromInternalValue(TValue value, TAccessor accessor) {
            return value;
        }

        protected override TValue ConvertToInternalValue(TValue value, TAccessor accessor) {
            return value;
        }
    }

    public abstract class KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> : IKeyValueStore<TKey, TValue, TAccessor, TInternalValue> {
        public TAccessor GetAccessor(TKey key) {
            return TryGetAccessor(key, out var result) ? result : throw CreateMissingMemberException(key);
        }

        public abstract bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TAccessor result);

        protected abstract MissingMemberException CreateMissingMemberException(TKey key);
        public TValue this[TKey key] {
            get {
                var accessor = GetAccessor(key);
                return ConvertFromInternalValue(this[accessor], accessor);
            }
            set {
                var accessor = GetAccessor(key);
                this[accessor] = ConvertToInternalValue(value, accessor);
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
            return ConvertFromInternalValue(GetValueOrInitialize(accessor), accessor);
        }

        public abstract bool ContainsAccessor(TAccessor accessor);

        public abstract Type GetDataType(TAccessor accessor);

        public abstract TInternalValue GetValueOrInitialize(TAccessor accessor);

        protected abstract TValue ConvertFromInternalValue(TInternalValue value, TAccessor accessor);

        protected abstract TInternalValue ConvertToInternalValue(TValue value, TAccessor accessor);
    }
}