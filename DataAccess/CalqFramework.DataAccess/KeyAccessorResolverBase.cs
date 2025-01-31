using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess {
    public abstract class KeyAccessorResolverBase<TKey, TValue> : IKeyAccessorResolver<TKey, TValue> {
        public TValue GetAccessor(TKey key) {
            return TryGetAccessor(key, out var result) ? result : throw CreateMissingMemberException(key);
        }

        public abstract bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TValue result);

        protected abstract MissingMemberException CreateMissingMemberException(TKey key);
    }
}
