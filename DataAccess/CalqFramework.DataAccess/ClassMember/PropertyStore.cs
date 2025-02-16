﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {

    public sealed class PropertyStore : PropertyStoreBase<string, object?>, IKeyValueStore<string, object?, PropertyInfo> {

        public PropertyStore(object obj, BindingFlags bindingFlags) : base(obj, bindingFlags) {
        }

        public override bool ContainsAccessor(PropertyInfo accessor) {
            return accessor is not null && accessor.DeclaringType == ParentType;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
            result = ParentType.GetProperty(key, BindingFlags);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(object? value, PropertyInfo accessor) {
            return value;
        }

        protected override object? ConvertToInternalValue(object? value, PropertyInfo accessor) {
            return value;
        }
    }
}
