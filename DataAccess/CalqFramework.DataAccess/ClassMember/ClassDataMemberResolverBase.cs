using CalqFramework.DataAccess;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class ClassDataMemberResolverBase<TKey, TValue> : IKeyAccessorResolver<TKey, MemberInfo> {
        protected object ParentObject { get; }
        protected Type ParentType { get; }
        protected BindingFlags BindingAttr { get; }

        public ClassDataMemberResolverBase(object obj, BindingFlags bindingAttr) {
            ParentObject = obj;
            BindingAttr = bindingAttr;
            ParentType = obj.GetType();
        }

        public bool ContainsKey(TKey key) {
            var result = GetClassMember(key);
            return result != null;
        }

        public bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        public MemberInfo GetAccessor(TKey key) {
            return GetClassMember(key) ?? throw new MissingMemberException($"Missing {key} in {ParentType}.");
        }

        protected abstract MemberInfo? GetClassMember(TKey key);
    }
}
