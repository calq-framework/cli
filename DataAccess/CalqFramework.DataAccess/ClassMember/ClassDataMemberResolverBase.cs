﻿using CalqFramework.DataAccess;
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
            return TryGetAccessor(key, out var _);
        }

        public abstract bool TryGetAccessor(TKey key, [MaybeNullWhen(false)] out MemberInfo result);

        public MemberInfo GetAccessor(TKey key) {
            return TryGetAccessor(key, out var result) ? result : throw new MissingMemberException($"Missing {key} in {ParentType}.");
        }
    }
}
