using System.Diagnostics.CodeAnalysis;

namespace CalqFramework.DataAccess {
    public interface IKeyAccessorResolver<TKey, TAccessor> {
        protected internal bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out TAccessor result);
        protected internal TAccessor GetAccessor(TKey key);
    }
}