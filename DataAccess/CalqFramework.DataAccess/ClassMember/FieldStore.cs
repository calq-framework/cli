﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    sealed public class FieldStore : FieldStoreBase<string, object?>, IKeyValueStore<string, object?, FieldInfo> {
        public FieldStore(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override bool ContainsAccessor(FieldInfo accessor) {
            return accessor is FieldInfo && accessor.DeclaringType == ParentType;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
            result = ParentType.GetField(key, BindingAttr);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(object? value, FieldInfo accessor) {
            return value;
        }

        protected override object? ConvertToInternalValue(object? value, FieldInfo accessor) {
            return value;
        }
    }
}